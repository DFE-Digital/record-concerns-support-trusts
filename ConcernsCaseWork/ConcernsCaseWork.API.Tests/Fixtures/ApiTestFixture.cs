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
using static System.Net.Mime.MediaTypeNames;

namespace ConcernsCaseWork.API.Tests.Fixtures
{
	public class ApiTestFixture : IDisposable
	{
		private readonly WebApplicationFactory<Startup> _application;
		private readonly DbContextOptions<ConcernsDbContext> _dbContextOptions;

		private readonly IConfiguration _initialConfig;

		public HttpClient Client { get; }
		public ServerUserInfoService ServerUserInfoService { get; }

		private static readonly object _lock = new();
		private static bool _isInitialized;

		private const string _connectionStringKey = "ConnectionStrings:DefaultConnection";
		private const string _tokenSetting = "Authorization:TokenSettings";
		private const string _policies = "Authorization:Policies";

		public ApiTestFixture()
		{
			lock (_lock)
			{
				if (!_isInitialized)
				{
					var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");
					var configBuilder = new ConfigurationBuilder()
						.AddJsonFile(configPath, optional: false, reloadOnChange: true)
						.AddEnvironmentVariables();

					_initialConfig = configBuilder.Build();

					var finalConnectionString = BuildDatabaseConnectionString(_initialConfig);

					var tokenSettings = _initialConfig.GetSection(_tokenSetting).Get<TokenSettings>();
					var policyList = _initialConfig.GetSection(_policies).Get<List<PolicyDefinition>>();

					var inMemoryOverrides = new Dictionary<string, string>
					{
						[_connectionStringKey] = finalConnectionString,
						[_tokenSetting] = JsonConvert.SerializeObject(tokenSettings),
						[_policies] = JsonConvert.SerializeObject(policyList)
					};

					_application = new WebApplicationFactory<Startup>()
						.WithWebHostBuilder(builder =>
						{
							builder.ConfigureAppConfiguration((context, config) =>
							{
								config.AddJsonFile(configPath, optional: false, reloadOnChange: true)
									  .AddEnvironmentVariables();

								config.AddInMemoryCollection(inMemoryOverrides);
							});

							builder.ConfigureServices(services =>
							{
								services.AddSingleton(GetMockMemoryCache());
								services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
								services.AddSingleton<IUserTokenService, UserTokenService>();

								services.AddSingleton<IOptions<TokenSettings>>(sp =>
								{
									var cfg = sp.GetRequiredService<IConfiguration>();
									var ts = cfg.GetSection(_tokenSetting).Get<TokenSettings>();
									return Options.Create(ts);
								});
							});
						});

					var fakeUserInfo = new UserInfo
					{
						Name = "API.TestFixture@test.gov.uk",
						Roles = [Claims.CaseWorkerRoleClaim, Claims.CaseDeleteRoleClaim]
					};
					ServerUserInfoService = new ServerUserInfoService { UserInfo = fakeUserInfo };

					Client = CreateHttpClient(fakeUserInfo);

					_dbContextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
						.UseSqlServer(finalConnectionString)
						.Options;

					using var context = GetContext();
					context.Database.EnsureDeleted();
					context.Database.Migrate();

					_isInitialized = true;
				}
			}
		}

		private static IMemoryCache GetMockMemoryCache()
		{
			var mockMemoryCache = new Mock<IMemoryCache>();
			var cacheEntry = Mock.Of<ICacheEntry>();

			mockMemoryCache
				.Setup(m => m.CreateEntry(It.IsAny<object>()))
				.Returns(cacheEntry);

			return mockMemoryCache.Object;
		}

		private HttpClient CreateHttpClient(UserInfo userInfo)
		{
			var client = _application.CreateClient();
			client.DefaultRequestHeaders.Add("ContentType", Application.Json);

			var clientUserInfoService = new ClientUserInfoService();
			clientUserInfoService.SetPrincipal(userInfo);
			clientUserInfoService.AddUserInfoRequestHeaders(client);

			var userTokenService = _application.Services.GetRequiredService<IUserTokenService>();
			var token = userTokenService.GetUserTokenAsync(GetClaimsPrincipal(userInfo)).Result;
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var correlationContext = new CorrelationContext();
			correlationContext.SetContext(Guid.NewGuid().ToString());

			AbstractService.AddDefaultRequestHeaders(client, correlationContext, clientUserInfoService, null);

			return client;
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

		private static string BuildDatabaseConnectionString(IConfiguration config)
		{
			var connection = config[_connectionStringKey];
			var sqlBuilder = new SqlConnectionStringBuilder(connection)
			{
				InitialCatalog = "ApiTests"
			};

			return sqlBuilder.ToString();
		}

		public ConcernsDbContext GetContext() => new ConcernsDbContext(_dbContextOptions, ServerUserInfoService);

		public void Dispose()
		{
			_application.Dispose();
			Client.Dispose();
		}
	}

}