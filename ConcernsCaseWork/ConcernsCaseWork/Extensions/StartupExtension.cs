using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Redis.Services;
using Service.TRAMS.Cases;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Net.Mime;

namespace ConcernsCaseWork.Extensions
{
	public static class StartupExtension
	{
		public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
		{
			// Check if we are running locally using user secrets
			var redisLocal = configuration["redis:local"] is { } && Boolean.Parse(configuration["redis:local"]);
			string host;
			string password;
			string port;
			bool tls;
			
			// Local feeds from user secrets configurations
			if (redisLocal)
			{
				host = configuration["redis:host"];
				password = configuration["redis:password"];
				port = configuration["redis:port"];
				tls = configuration["redis:tls"] is { } && Boolean.Parse(configuration["redis:tls"]);
			}
			else
			{
				var vcapConfiguration = JObject.Parse(configuration["VCAP_SERVICES"]);
				if (vcapConfiguration is null) 
					throw new ConfigurationErrorsException("AddRedis::VCAP_SERVICES missing");
				
				var redisCredentials = vcapConfiguration["redis"]?[0]?["credentials"];
				if (redisCredentials is null) 
					throw new ConfigurationErrorsException("AddRedis::redisCredentials missing");
				
				host = (string) redisCredentials["host"];
				password = (string) redisCredentials["password"];
				port = (string) redisCredentials["port"];
				tls = (bool) redisCredentials["tls_enabled"];
			}
			
			Log.Information($"Starting Redis Server Host - {host}");
			Log.Information($"Starting Redis Server Port - {port}");
			Log.Information($"Starting Redis Server TLS - {tls}");
			
			var redisConfigurationOptions = new ConfigurationOptions
			{
				Password = password,
				EndPoints = {$"{host}:{port}"},
				Ssl = tls
			};
			var redisConnection = ConnectionMultiplexer.Connect(redisConfigurationOptions);
            
			services.AddStackExchangeRedisCache(
				options =>
				{
					options.ConfigurationOptions = redisConfigurationOptions;
					options.InstanceName = $"Redis-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
				});
			services.AddDataProtection().PersistKeysToStackExchangeRedis(redisConnection, "DataProtectionKeys");
		}

		/// <summary>
		/// HttpFactory for Trams API
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <exception cref="ConfigurationErrorsException"></exception>
		public static void AddTramsApi(this IServiceCollection services, IConfiguration configuration)
		{
			var tramsApiEndpoint = configuration["trams_api_endpoint"];
			var tramsApiKey = configuration["trams_api_key"];
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
			// Cases service model and external TRAMS.
			services.AddSingleton<ICaseModelService, CaseModelService>();
			services.AddSingleton<ICaseService, CaseService>();
			
			// Redis service model
			services.AddTransient<ICacheProvider, CacheProvider>();
			services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();
			services.AddTransient<ICachedUserService, CachedUserService>();
		}
	}
}