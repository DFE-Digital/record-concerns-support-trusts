using ConcernsCaseWork.Data;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Fixtures
{
	public class ApiTestFixture : IDisposable
	{
		private readonly WebApplicationFactory<Startup> _application;

		public HttpClient Client { get; init; }

		private DbContextOptions<ConcernsDbContext> _dbContextOptions { get; init; }

		private ServerUserInfoService _serverUserInfoService { get; init; }

		private static readonly object _lock = new();
		private static bool _isInitialised = false;

		private const string ConnectionStringKey = "ConnectionStrings:DefaultConnection";

		public ApiTestFixture()
		{
			lock (_lock)
			{
				if (!_isInitialised)
				{
					string connectionString = null;

					_application = new WebApplicationFactory<Startup>()
						.WithWebHostBuilder(builder =>
						{
							var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");

							builder.ConfigureAppConfiguration((context, config) =>
							{
								config.AddJsonFile(configPath)
									.AddEnvironmentVariables();

								connectionString = BuildDatabaseConnectionString(config);

								config.AddInMemoryCollection(new Dictionary<string, string>
								{
									[ConnectionStringKey] = connectionString
								});
							});
						});

					var fakeUserInfo = new UserInfo()
						{ Name = "API.TestFixture@test.gov.uk", Roles = new[] { Claims.CaseWorkerRoleClaim } };
					_serverUserInfoService = new ServerUserInfoService() { UserInfo = fakeUserInfo };

					Client = CreateHttpClient(fakeUserInfo);

					_dbContextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
						.UseSqlServer(connectionString)
						.Options;

					using var context = GetContext();
					context.Database.EnsureDeleted();
					context.Database.Migrate();
					_isInitialised = true;
				}
			}
		}

		private HttpClient CreateHttpClient(UserInfo userInfo)
		{
			var client = _application.CreateClient();
			client.DefaultRequestHeaders.Add("ApiKey", "app-key");
			client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);

			// add standard headers for correlation and user context.
			var clientUserInfoService = new ClientUserInfoService();
			clientUserInfoService.SetPrincipal(userInfo);
			clientUserInfoService.AddUserInfoRequestHeaders(client);

			var correlationContext = new CorrelationContext();
			correlationContext.SetContext(Guid.NewGuid().ToString());

			AbstractService.AddDefaultRequestHeaders(client, correlationContext, clientUserInfoService, null);

			return client;
		}

		private static string BuildDatabaseConnectionString(IConfigurationBuilder config)
		{
			var currentConfig = config.Build();
			var connection = currentConfig[ConnectionStringKey];
			var sqlBuilder = new SqlConnectionStringBuilder(connection);
			sqlBuilder.InitialCatalog = "ApiTests";

			var result = sqlBuilder.ToString();

			return result;
		}

		public ConcernsDbContext GetContext() => new ConcernsDbContext(_dbContextOptions, _serverUserInfoService);

		public void Dispose()
		{
			_application.Dispose();
			Client.Dispose();
		}
	}

	[CollectionDefinition(ApiTestCollectionName)]
	public class ApiTestCollection : ICollectionFixture<ApiTestFixture>
	{
		public const string ApiTestCollectionName = "ApiTestCollection";

		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}