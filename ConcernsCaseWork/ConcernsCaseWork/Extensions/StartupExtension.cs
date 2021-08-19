using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
			var vcapConfiguration = configuration.GetSection("VCAP_SERVICES:redis:0");
			if (vcapConfiguration is null) 
				throw new ConfigurationErrorsException("AddRedis::vcapConfiguration::redis:0");
            
			var redisCredentials = vcapConfiguration.GetSection("credentials");
			if (redisCredentials is null) 
				throw new ConfigurationErrorsException("AddRedis::redisCredentials::credentials");
			
			Log.Information($"Starting Redis Server Host - {redisCredentials["host"]}");
			Log.Information($"Starting Redis Server Port - {redisCredentials["port"]}");
			Log.Information($"Starting Redis Server TLS - {redisCredentials["tls_enabled"]}");
			
			var redisConfigurationOptions = new ConfigurationOptions
			{
				Password = redisCredentials["password"],
				EndPoints = {$"{redisCredentials["host"]}:{redisCredentials["port"]}"},
				Ssl = redisCredentials["tls_enabled"] is { } && Boolean.Parse(redisCredentials["tls_enabled"])
			};
			var redisConnection = ConnectionMultiplexer.Connect(redisConfigurationOptions);
            
			services.AddStackExchangeRedisCache(
				options => { options.ConfigurationOptions = redisConfigurationOptions; });
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
			var tramsApiEndpoint = configuration["TRAMS_API_ENDPOINT"];
			var tramsApiKey = configuration["TRAMS_API_KEY"];
			if (string.IsNullOrEmpty(tramsApiEndpoint) || string.IsNullOrEmpty(tramsApiKey)) 
				throw new ConfigurationErrorsException("AddTramsApi::missing configuration");
			
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