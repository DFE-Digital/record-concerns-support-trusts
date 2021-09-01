using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Redis.Base;
using Service.Redis.Configuration;
using Service.Redis.Users;
using Service.TRAMS.Cases;
using Service.TRAMS.Configuration;
using Service.TRAMS.Trusts;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Net.Mime;

namespace ConcernsCaseWork.Extensions
{
	public static class StartupExtension
	{
		private static readonly IRedisMultiplexer RedisMultiplexer = new RedisMultiplexer();
		public static IRedisMultiplexer Implementation { private get; set; } = RedisMultiplexer;
		
		public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
		{
			try
			{
				var vCapConfiguration = JObject.Parse(configuration["VCAP_SERVICES"]) ?? throw new ConfigurationErrorsException("AddRedis::VCAP_SERVICES missing");
				var redisCredentials = vCapConfiguration["redis"]?[0]?["credentials"] ?? throw new ConfigurationErrorsException("AddRedis::Credentials missing");
				var password = (string)redisCredentials["password"] ?? throw new ConfigurationErrorsException("AddRedis::Credentials::password missing");
				var host = (string)redisCredentials["host"] ?? throw new ConfigurationErrorsException("AddRedis::Credentials::host missing");
				var port = (string)redisCredentials["port"] ?? throw new ConfigurationErrorsException("AddRedis::Credentials::port missing");
				var tls = (bool)redisCredentials["tls_enabled"];

				Log.Information($"Starting Redis Server Host - {host}");
				Log.Information($"Starting Redis Server Port - {port}");
				Log.Information($"Starting Redis Server TLS - {tls}");

				var redisConfigurationOptions = new ConfigurationOptions { Password = password, EndPoints = { $"{host}:{port}" }, Ssl = tls };
				var redisConnection = Implementation.Connect(redisConfigurationOptions);

				services.AddStackExchangeRedisCache(
					options =>
					{
						options.ConfigurationOptions = redisConfigurationOptions;
						options.InstanceName = $"Redis-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
					});
				services.AddDataProtection().PersistKeysToStackExchangeRedis(redisConnection, "DataProtectionKeys");
			}
			catch (Exception ex)
			{
				var errorMessage = $"AddRedis::Exception::{ex.Message}";
				Log.Error(errorMessage);
				throw new ConfigurationErrorsException(errorMessage);
			}
		}

		/// <summary>
		/// HttpFactory for Trams API
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <exception cref="ConfigurationErrorsException"></exception>
		public static void AddTramsApi(this IServiceCollection services, IConfiguration configuration)
		{
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			var tramsApiKey = configuration["trams:api_key"];
			if (string.IsNullOrEmpty(tramsApiEndpoint) || string.IsNullOrEmpty(tramsApiKey)) 
				throw new ConfigurationErrorsException("AddTramsApi::missing configuration");
			
			Log.Information($"Starting Trams API Endpoint - {tramsApiEndpoint}");
			
			services.AddHttpClient("TramsClient", client =>
			{
				client.BaseAddress = new Uri(tramsApiEndpoint);
				client.DefaultRequestHeaders.Add("ApiKey", tramsApiKey);
				client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
			});
		}

		public static void AddInternalServices(this IServiceCollection services)
		{
			// Web application services
			services.AddSingleton<ICaseModelService, CaseModelService>();
			services.AddSingleton<ITrustModelService, TrustModelService>();
			
			// Trams api services
			services.AddSingleton<ICaseService, CaseService>();
			services.AddSingleton<ITrustService, TrustService>();
			services.AddSingleton<ITrustSearchService, TrustSearchService>();
			
			// Redis services
			services.AddTransient<ICacheProvider, CacheProvider>();
			services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();
			services.AddTransient<IUserCachedService, UserCachedService>();
		}

		public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache));
			services.Configure<TrustSearchOptions>(configuration.GetSection(TrustSearchOptions.Cache));
		}
	}
}