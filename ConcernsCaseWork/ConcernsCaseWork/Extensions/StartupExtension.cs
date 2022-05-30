using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Configuration;
using Service.Redis.FinancialPlan;
using Service.Redis.Ratings;
using Service.Redis.Records;
using Service.Redis.Security;
using Service.Redis.Sequence;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
using Service.Redis.Users;
using Service.Redis.CaseActions;
using Service.TRAMS.Cases;
using Service.TRAMS.Configuration;
using Service.TRAMS.FinancialPlan;
using Service.TRAMS.Ratings;
using Service.TRAMS.RecordAcademy;
using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Records;
using Service.TRAMS.RecordSrma;
using Service.TRAMS.RecordWhistleblower;
using Service.TRAMS.Status;
using Service.TRAMS.Trusts;
using Service.TRAMS.Types;
using StackExchange.Redis;
using System;
using System.Net.Mime;
using Service.TRAMS.CaseActions;

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
				var vCapConfiguration = JObject.Parse(configuration["VCAP_SERVICES"]) ?? throw new Exception("AddRedis::VCAP_SERVICES missing");
				var redisCredentials = vCapConfiguration["redis"]?[0]?["credentials"] ?? throw new Exception("AddRedis::Credentials missing");
				var password = (string)redisCredentials["password"] ?? throw new Exception("AddRedis::Credentials::password missing");
				var host = (string)redisCredentials["host"] ?? throw new Exception("AddRedis::Credentials::host missing");
				var port = (string)redisCredentials["port"] ?? throw new Exception("AddRedis::Credentials::port missing");
				var tls = (bool)redisCredentials["tls_enabled"];

				Log.Information("Starting Redis Server Host - {Host}", host);
				Log.Information("Starting Redis Server Port - {Port}", port);
				Log.Information("Starting Redis Server TLS - {Tls}", tls);

				var redisConfigurationOptions = new ConfigurationOptions { Password = password, EndPoints = { $"{host}:{port}" }, Ssl = tls, AsyncTimeout = 15000 };
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
				Log.Error("AddRedis::Exception::{Message}", ex.Message);
				throw new Exception($"AddRedis::Exception::{ex.Message}");
			}
		}

		/// <summary>
		/// HttpFactory for Trams API
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <exception cref="Exception"></exception>
		public static void AddTramsApi(this IServiceCollection services, IConfiguration configuration)
		{
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			var tramsApiKey = configuration["trams:api_key"];

			if (string.IsNullOrEmpty(tramsApiEndpoint) || string.IsNullOrEmpty(tramsApiKey))
				throw new Exception("AddTramsApi::missing configuration");

			Log.Information("Starting Trams API Endpoint - {TramsApiEndpoint}", tramsApiEndpoint);

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
			services.AddSingleton<ITypeModelService, TypeModelService>();
			services.AddSingleton<ICaseHistoryModelService, CaseHistoryModelService>();
			services.AddSingleton<IRatingModelService, RatingModelService>();
			services.AddSingleton<IRecordModelService, RecordModelService>();
			services.AddSingleton<IFinancialPlanModelService, FinancialPlanModelService>();
			services.AddSingleton<ISRMAService, SRMAService>();

			// Trams api services
			services.AddSingleton<ICaseService, CaseService>();
			services.AddSingleton<IRatingService, RatingService>();
			services.AddSingleton<IRecordAcademyService, RecordAcademyService>();
			services.AddSingleton<IRecordRatingHistoryService, RecordRatingHistoryService>();
			services.AddSingleton<IRecordService, RecordService>();
			services.AddSingleton<IRecordSrmaService, RecordSrmaService>();
			services.AddSingleton<IRecordWhistleblowerService, RecordWhistleblowerService>();
			services.AddSingleton<IStatusService, StatusService>();
			services.AddSingleton<ITrustService, TrustService>();
			services.AddSingleton<ITrustSearchService, TrustSearchService>();
			services.AddSingleton<ITypeService, TypeService>();
			services.AddSingleton<ICaseSearchService, CaseSearchService>();
			services.AddSingleton<ICaseHistoryService, CaseHistoryService>();
			services.AddSingleton<IFinancialPlanService, FinancialPlanService>();
			services.AddSingleton<SRMAProvider, SRMAProvider>();
			services.AddSingleton<IFinancialPlanStatusService, FinancialPlanStatusService>();

			// Redis services
			services.AddSingleton<ICacheProvider, CacheProvider>();
			services.AddSingleton<ICachedService, CachedService>();
			services.AddSingleton<ITypeCachedService, TypeCachedService>();
			services.AddSingleton<IStatusCachedService, StatusCachedService>();
			services.AddSingleton<IRatingCachedService, RatingCachedService>();
			services.AddSingleton<ITrustCachedService, TrustCachedService>();
			services.AddTransient<ICaseCachedService, CaseCachedService>();
			services.AddTransient<IRecordCachedService, RecordCachedService>();
			services.AddSingleton<ICaseHistoryCachedService, CaseHistoryCachedService>();
			services.AddSingleton<IFinancialPlanCachedService, FinancialPlanCachedService>();
			services.AddSingleton<IFinancialPlanStatusCachedService, FinancialPlanStatusCachedService>();
			
			services.AddSingleton<CachedSRMAProvider, CachedSRMAProvider>();

			// AD Integration
			services.AddSingleton<IActiveDirectoryService, ActiveDirectoryService>();
			services.AddSingleton<IUserRoleCachedService, UserRoleCachedService>();
			services.AddSingleton<IRbacManager, RbacManager>();

			// Redis Sequence
			services.AddSingleton<ISequenceCachedService, SequenceCachedService>();
		}

		public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache));
			services.Configure<TrustSearchOptions>(configuration.GetSection(TrustSearchOptions.Cache));
		}
	}
}