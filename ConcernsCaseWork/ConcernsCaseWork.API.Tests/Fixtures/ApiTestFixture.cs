using ConcernsCaseWork.Data;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Configurations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Xunit;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
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

		private TokenSettings GetTokenSettings()
		{
			var configuration = _application.Services.GetRequiredService<IConfiguration>();
			return configuration.GetSection(_tokenSetting).Get<TokenSettings>();
		}

		private HttpClient CreateHttpClient(UserInfo userInfo)
		{
			var client = _application.CreateClient(); 
			client.DefaultRequestHeaders.Add("ContentType", Application.Json); 
			// add standard headers for correlation and user context.
			var clientUserInfoService = new ClientUserInfoService();
			clientUserInfoService.SetPrincipal(userInfo);
			clientUserInfoService.AddUserInfoRequestHeaders(client);

			var token = GenerateToken(userInfo, GetTokenSettings());
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			
			var correlationContext = new CorrelationContext();
			correlationContext.SetContext(Guid.NewGuid().ToString());
			
			AbstractService.AddDefaultRequestHeaders(client, correlationContext, clientUserInfoService, null);

			return client;
		}

		private string GenerateToken(UserInfo user, TokenSettings tokenSettings)
		{ 
			List<Claim> claims =
			[
				new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Name ?? string.Empty),
			];
			foreach (var role in user.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			DateTime? expires = DateTime.UtcNow.AddMinutes(tokenSettings.TokenLifetimeMinutes);
			JwtSecurityToken token = new(tokenSettings.Issuer, tokenSettings.Audience, claims, null, expires, new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)), "HS256"));
			return new JwtSecurityTokenHandler().WriteToken(token);
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

	[CollectionDefinition(_apiTestCollectionName)]
	public class ApiTestCollection : ICollectionFixture<ApiTestFixture>
	{
		public const string _apiTestCollectionName = "ApiTestCollection";

		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}