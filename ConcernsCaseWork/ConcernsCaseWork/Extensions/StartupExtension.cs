using ConcernsCaseWork.API.StartupConfiguration;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.CaseActions;
using ConcernsCaseWork.Redis.Configuration;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Redis.MeansOfReferral;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Service.CaseActions;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Configuration;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Service.MeansOfReferral;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Service.Teams;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Service.Types;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.PageHistory;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Teams;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using StackExchange.Redis;
using System;
using System.Net.Mime;

namespace ConcernsCaseWork.Extensions
{
	public static class StartupExtension
	{
		private static readonly IRedisMultiplexer _redisMultiplexer = new RedisMultiplexer();
		public static IRedisMultiplexer RedisMultiplexerImplementation { private get; set; } = _redisMultiplexer;

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
				var redisConnection = RedisMultiplexerImplementation.Connect(redisConfigurationOptions);

				services.AddStackExchangeRedisCache(
					options =>
					{
						options.ConfigurationOptions = redisConfigurationOptions;
						options.InstanceName = $"Redis-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
					});

				services.AddDataProtection().PersistKeysToStackExchangeRedis(redisConnection, "DataProtectionKeys");

				services.AddSingleton<IConnectionMultiplexer>(redisConnection);
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

		/// <summary>
		/// HttpFactory for Concerns API
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <exception cref="Exception"></exception>
		public static void AddConcernsApi(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddConcernsApiProject(configuration);
		}

		public static void AddInternalServices(this IServiceCollection services)
		{
			// Web application services
			services.AddScoped<ICaseModelService, CaseModelService>();
			services.AddScoped<ITrustModelService, TrustModelService>();
			services.AddScoped<ITypeModelService, TypeModelService>();
			services.AddScoped<IRatingModelService, RatingModelService>();
			services.AddScoped<IRecordModelService, RecordModelService>();
			services.AddScoped<IFinancialPlanModelService, FinancialPlanModelService>();
			services.AddScoped<ISRMAService, SRMAService>();
			services.AddScoped<INtiUnderConsiderationReasonsCachedService, NtiUnderConsiderationReasonsCachedService>();
			services.AddScoped<INtiUnderConsiderationModelService, NtiUnderConsiderationModelService>();
			services.AddScoped<INtiWarningLetterModelService, NtiWarningLetterModelService>();
			services.AddScoped<IMeansOfReferralModelService, MeansOfReferralModelService>();
			services.AddScoped<INtiModelService, NtiModelService>();
			services.AddScoped<IActionsModelService, ActionsModelService>();
			services.AddScoped<ITeamsModelService, TeamsModelService>();
			services.AddScoped<IClaimsPrincipalHelper, ClaimsPrincipalHelper>();
			services.AddScoped<ICaseActionValidationStrategy, FinancialPanValidator>();
			services.AddScoped<ICaseActionValidationStrategy, SRMAValidator>();
			services.AddScoped<ICaseActionValidationStrategy, NTIUnderConsiderationValidator>();
			services.AddScoped<ICaseActionValidationStrategy, NTIWarningLetterValidator>();
			services.AddScoped<ICaseActionValidationStrategy, NTIValidator>();
			services.AddScoped<ICaseActionValidationStrategy, DecisionValidator>();
			services.AddScoped<ICaseActionValidator, CaseActionValidator>();
			services.AddScoped<IDecisionModelService, DecisionModelService>();
			services.AddScoped<ICreateCaseService, CreateCaseService>();
			services.AddScoped<ICaseSummaryService, CaseSummaryService>();
			services.AddScoped<IApiCaseSummaryService, ApiCaseSummaryService>();

			// api services
			services.AddScoped<ICaseService, CaseService>();
			services.AddScoped<IRatingService, RatingService>();
			services.AddScoped<IRecordService, RecordService>();
			services.AddScoped<IStatusService, StatusService>();
			services.AddScoped<ITrustService, TrustService>();
			services.AddScoped<ITrustSearchService, TrustSearchService>();
			services.AddScoped<ITypeService, TypeService>();
			services.AddScoped<ICaseSearchService, CaseSearchService>();
			services.AddScoped<IFinancialPlanService, FinancialPlanService>();
			services.AddScoped<SRMAProvider, SRMAProvider>();
			services.AddScoped<IFinancialPlanStatusService, FinancialPlanStatusService>();
			services.AddScoped<INtiUnderConsiderationReasonsService, NtiUnderConsiderationReasonsService>();
			services.AddScoped<INtiUnderConsiderationStatusesService, NtiUnderConsiderationStatusesService>();
			services.AddScoped<INtiUnderConsiderationService, NtiUnderConsiderationService>();
			services.AddScoped<IMeansOfReferralService, MeansOfReferralService>();
			services.AddScoped<INtiWarningLetterStatusesService, NtiWarningLetterStatusesService>();
			services.AddScoped<INtiWarningLetterReasonsService, NtiWarningLetterReasonsService>();
			services.AddScoped<INtiWarningLetterService, NtiWarningLetterService>();
			services.AddScoped<INtiWarningLetterConditionsService, NtiWarningLetterConditionsService>();
			services.AddScoped<INtiService, NtiService>();
			services.AddScoped<INtiStatusesService, NtiStatusesService>();
			services.AddScoped<INtiReasonsService, NtiReasonsService>();
			services.AddScoped<INtiConditionsService, NtiConditionsService>();
			services.AddScoped<ITeamsService, TeamsService>();
			services.AddScoped<IDecisionService, DecisionService>();
			services.AddScoped<ICasePermissionsService, CasePermissionsService>();

			// Redis services
			services.AddSingleton<ICacheProvider, CacheProvider>();
			services.AddScoped<IUserStateCachedService, UserStateCachedService>();
			services.AddScoped<ITypeCachedService, TypeCachedService>();
			services.AddScoped<IStatusCachedService, StatusCachedService>();
			services.AddScoped<IRatingCachedService, RatingCachedService>();
			services.AddScoped<ITrustCachedService, TrustCachedService>();
			services.AddScoped<IFinancialPlanStatusCachedService, FinancialPlanStatusCachedService>();
			services.AddScoped<CachedSRMAProvider, CachedSRMAProvider>();
			services.AddScoped<INtiUnderConsiderationReasonsCachedService, NtiUnderConsiderationReasonsCachedService>();
			services.AddScoped<INtiUnderConsiderationStatusesCachedService, NtiUnderConsiderationStatusesCachedService>();
			services.AddScoped<INtiUnderConsiderationCachedService, NtiUnderConsiderationCachedService>();
			services.AddScoped<INtiWarningLetterStatusesCachedService, NtiWarningLetterStatusesCachedService>();
			services.AddScoped<INtiWarningLetterReasonsCachedService, NtiWarningLetterReasonsCachedService>();
			services.AddScoped<INtiWarningLetterCachedService, NtiWarningLetterCachedService>();
			services.AddScoped<IMeansOfReferralCachedService, MeansOfReferralCachedService>();
			services.AddScoped<INtiWarningLetterConditionsCachedService, NtiWarningLetterConditionsCachedServices>();
			services.AddScoped<INtiCachedService, NtiCachedService>();
			services.AddScoped<INtiStatusesCachedService, NtiStatusesCachedService>();
			services.AddScoped<INtiReasonsCachedService, NtiReasonsCachedService>();
			services.AddScoped<INtiConditionsCachedService, NtiConditionsCachedService>();
			services.AddScoped<ITeamsCachedService, TeamsCachedService>();
			services.AddScoped<ICaseSummaryService, CaseSummaryService>();

			// Redis Sequence
			// TODO. This class looks very temporary. What's it for and how are we going to replace it.

			// AD Integration
			services.AddScoped<IRbacManager, RbacManager>();

			services.AddScoped<ICorrelationContext, CorrelationContext>();

			services.AddHttpContextAccessor();
			services.AddScoped<IRbacManager, RbacManager>();

			services.AddScoped<ICorrelationContext, CorrelationContext>();
			services.AddScoped<IClientUserInfoService, ClientUserInfoService>();
			services.AddSingleton<IPageHistoryStorageHandler, SessionPageHistoryStorageHandler>();
		}

		public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache));
			services.Configure<TrustSearchOptions>(configuration.GetSection(TrustSearchOptions.Cache));
		}
	}
}