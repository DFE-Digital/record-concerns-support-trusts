using ConcernsCaseWork.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Fixtures
{
	public class ApiTestFixture : IDisposable
	{
		private WebApplicationFactory<Startup> _application;

		public HttpClient Client { get; private set; }

		private DbContextOptions<ConcernsDbContext> _dbContextOptions { get; init; }

		private readonly object _lock = new();
		private bool _isInitialised = false;

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

					Client = _application.CreateClient();
					Client.DefaultRequestHeaders.Add("ApiKey", "app-key");

					_dbContextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
						.UseSqlServer(connectionString)
						.Options;

					using var context = GetContext();
					context.Database.Migrate();
					_isInitialised = true;
				}
			}
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

		public ConcernsDbContext GetContext() => new ConcernsDbContext(_dbContextOptions);

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