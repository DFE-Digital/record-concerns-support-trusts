﻿using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
			if (vcapConfiguration is null) throw new ConfigurationErrorsException("AddRedis::vcapConfiguration::redis:0");
            
			var redisCredentials = vcapConfiguration.GetSection("credentials");
			if (redisCredentials is null) throw new ConfigurationErrorsException("AddRedis::redisCredentials::credentials");
			
			var redisConfigurationOptions = new ConfigurationOptions()
			{
				Password = redisCredentials?["password"],
				EndPoints = {$"{redisCredentials["host"]}:{redisCredentials["port"]}"},
				Ssl = Boolean.Parse(redisCredentials["tls_enabled"])
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
			var tramsApiBase = configuration["TRAMS_API_ENDPOINT"];
			var tramsApiKey = configuration["TRAMS_API_KEY"];
			if (string.IsNullOrEmpty(tramsApiBase) || string.IsNullOrEmpty(tramsApiKey)) throw new ConfigurationErrorsException("AddTramsApi::missing configuration");
			
			services.AddHttpClient("TramsClient", client =>
			{
				var apiKey = configuration["TRAMS_API_ENDPOINT"];
				var endpoint = configuration["TRAMS_API_KEY"];
	            
				client.BaseAddress = new Uri(endpoint);
				client.DefaultRequestHeaders.Add("ApiKey", apiKey);
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
			services.AddTransient<ICacheUserService, CacheUserService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IUserService, CachedUserService>();
		}
	}
}