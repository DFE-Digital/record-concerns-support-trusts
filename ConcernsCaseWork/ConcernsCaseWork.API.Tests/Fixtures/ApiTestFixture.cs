using ConcernsCaseWork.Data;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Authorization;
using DfE.CoreLibs.Security.Configurations;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace ConcernsCaseWork.API.Tests.Fixtures
{
	public class ApiTestFixture : IDisposable
	{
		private readonly WebApplicationFactory<Startup> _application;

		public HttpClient Client { get; init; }

		private DbContextOptions<ConcernsDbContext> _dbContextOptions { get; init; }

		private ServerUserInfoService ServerUserInfoService { get; init; }

		private static readonly object _lock = new();
		private static bool _isInitialised = false;

		private const string _connectionStringKey = "ConnectionStrings:DefaultConnection";
		private const string _tokenSetting = "Authorization:TokenSettings";
		private const string _policies = "Authorization:Policies";

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
									[_connectionStringKey] = connectionString, 
									[_tokenSetting] = JsonConvert.SerializeObject(GetAppSettings<TokenSettings>(config, _tokenSetting)),
									[_policies] = JsonConvert.SerializeObject(GetAppSettings<List<PolicyDefinition>>(config, _policies))
								}); 
							});
							builder.ConfigureServices(services =>
							{
								services.AddSingleton(GetTokenSettings());
								services.AddSingleton(GetMemoryCache());
								services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
								services.AddSingleton<IUserTokenService, UserTokenService>();
							});
						}); 

					var fakeUserInfo = new UserInfo()
					{ 
						Name = "API.TestFixture@test.gov.uk",
						Roles = [Claims.CaseWorkerRoleClaim, Claims.CaseDeleteRoleClaim] 
						
					};
					ServerUserInfoService = new ServerUserInfoService() { UserInfo = fakeUserInfo };
					
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
		private static IMemoryCache GetMemoryCache()
		{
			var mockMemoryCache = new Mock<IMemoryCache>();

			// Set up the mock behavior
			string key = "testKey";
			object expectedValue = "testValue";

			mockMemoryCache
				.Setup(m => m.TryGetValue(key, out expectedValue))
				.Returns(true);

			mockMemoryCache
				.Setup(m => m.CreateEntry(It.IsAny<object>()))
				.Returns(Mock.Of<ICacheEntry>);
			return mockMemoryCache.Object;
		}

		private static ClaimsPrincipal GetClaimsPrincipal(UserInfo user)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, user.Name),
				new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Name)
			};
			foreach (var role in user.Roles)
			{
				claims.Add(new(ClaimTypes.Role, role));
			}
			return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthenticationType"));
		}

		private IOptions<TokenSettings> GetTokenSettings()
		{
			var configuration = _application.Services.GetRequiredService<IConfiguration>();
			var mockTokenSettingsOption = new Mock<IOptions<TokenSettings>>();
			mockTokenSettingsOption.Setup(x => x.Value).Returns(configuration.GetSection(_tokenSetting).Get<TokenSettings>());
			return mockTokenSettingsOption.Object;
		}

		private HttpClient CreateHttpClient(UserInfo userInfo)
		{
			var client = _application.CreateClient(); 
			client.DefaultRequestHeaders.Add("ContentType", Application.Json); 
			// add standard headers for correlation and user context.
			var clientUserInfoService = new ClientUserInfoService();
			clientUserInfoService.SetPrincipal(userInfo);
			clientUserInfoService.AddUserInfoRequestHeaders(client);

			var userToken = _application.Services.GetService<IUserTokenService>();

			var token = userToken.GetUserTokenAsync(GetClaimsPrincipal(userInfo)).Result;
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			
			var correlationContext = new CorrelationContext();
			correlationContext.SetContext(Guid.NewGuid().ToString());
			
			AbstractService.AddDefaultRequestHeaders(client, correlationContext, clientUserInfoService, null);

			return client;
		}

		private static string BuildDatabaseConnectionString(IConfigurationBuilder config)
		{
			var currentConfig = config.Build();
			var connection = currentConfig[_connectionStringKey];
			var sqlBuilder = new SqlConnectionStringBuilder(connection)
			{
				InitialCatalog = "ApiTests"
			};

			var result = sqlBuilder.ToString();

			return result;
		}

		private static T GetAppSettings<T>(IConfigurationBuilder config, string appSettingSection)
		{
			var currentConfig = config.Build();
			return currentConfig.Get<T>(); 
		}

		public ConcernsDbContext GetContext() => new(_dbContextOptions, ServerUserInfoService);

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