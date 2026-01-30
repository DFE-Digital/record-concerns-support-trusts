using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.API.StartupConfiguration;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Configuration;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.AzureAd.Factories;
using ConcernsCaseWork.Service.AzureAd.Options;
using ConcernsCaseWork.Service.AzureAd.Services;
using ConcernsCaseWork.Service.CaseActions;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
using ConcernsCaseWork.Service.Teams;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.PageHistory;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Teams;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Extensions
{
	public static class StartupExtension
	{
		private static IConnectionMultiplexer _redisConnectionMultiplexer;
		//private static ILogger _logger;


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

				/*_logger.LogInformation("Starting Redis Server Host - {Host}", host);
				_logger.LogInformation("Starting Redis Server Port - {Port}", port);
				_logger.LogInformation("Starting Redis Server TLS - {Tls}", tls);*/

				var redisConfigurationOptions = new ConfigurationOptions { Password = password, EndPoints = { $"{host}:{port}" }, Ssl = tls, AsyncTimeout = 15000, SyncTimeout = 15000 };

				var preventThreadTheftStr = configuration["PreventRedisThreadTheft"] ?? "false";
				if (bool.TryParse(preventThreadTheftStr, out bool preventThreadTheft) && preventThreadTheft)
				{
					// https://stackexchange.github.io/StackExchange.Redis/ThreadTheft.html
					ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
				}

				_redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
				services.AddDataProtection().PersistKeysToStackExchangeRedis(_redisConnectionMultiplexer, "DataProtectionKeys");

				services.AddStackExchangeRedisCache(
					options =>
					{
						options.ConfigurationOptions = redisConfigurationOptions;
						options.InstanceName = $"Redis-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
						options.ConnectionMultiplexerFactory = () => Task.FromResult(_redisConnectionMultiplexer);
					});

				services.AddHealthChecks().AddRedis(_redisConnectionMultiplexer);
			}
			catch (Exception ex)
			{
				// _logger.LogError("AddRedis::Exception::{Message}", ex.Message);
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

			//_logger.LogInformation("Starting Trams API Endpoint - {TramsApiEndpoint}", tramsApiEndpoint);

			services.AddHttpClient("TramsClient", client =>
			{
				client.BaseAddress = new Uri(tramsApiEndpoint);
				client.DefaultRequestHeaders.Add("ApiKey", tramsApiKey);
				client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
				client.DefaultRequestHeaders.Add("User-Agent", "RecordConcernsSupportTrusts/1.0");
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
			services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();
			services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
			// Web application services
			services.AddScoped<ICaseModelService, CaseModelService>();
			services.AddScoped<ITrustModelService, TrustModelService>();
			services.AddScoped<IRecordModelService, RecordModelService>();
			services.AddScoped<IFinancialPlanModelService, FinancialPlanModelService>();
			services.AddScoped<ISRMAService, SRMAService>();
			services.AddScoped<INtiUnderConsiderationModelService, NtiUnderConsiderationModelService>();
			services.AddScoped<INtiWarningLetterModelService, NtiWarningLetterModelService>();
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
			services.AddScoped<ICaseActionValidationStrategy, TrustFinancialForecastValidator>();
			services.AddScoped<ICaseActionValidationStrategy, TargetedTrustEngagementValidator>();
			services.AddScoped<ICaseActionValidator, CaseActionValidator>();
			services.AddScoped<IDecisionModelService, DecisionModelService>();
			services.AddScoped<ICreateCaseService, CreateCaseService>();
			services.AddScoped<ICaseSummaryService, CaseSummaryService>();
			services.AddScoped<IApiCaseSummaryService, ApiCaseSummaryService>();
			services.AddScoped<ICaseValidatorService, CaseValidatorService>();

			services.AddScoped<IGraphClientFactory, GraphClientFactory>();
			services.AddScoped<IGraphUserService, GraphUserService>();

			// api services
			services.AddScoped<ICaseService, CaseService>();
			services.AddScoped<IRecordService, RecordService>();
			services.AddScoped<ITrustService, TrustService>();
			services.AddScoped<ITrustSearchService, TrustSearchService>();
			services.AddScoped<ICaseSearchService, CaseSearchService>();
			services.AddScoped<IFinancialPlanService, FinancialPlanService>();
			services.AddScoped<SRMAProvider, SRMAProvider>();
			services.AddScoped<INtiUnderConsiderationService, NtiUnderConsiderationService>();
			services.AddScoped<INtiWarningLetterService, NtiWarningLetterService>();
			services.AddScoped<INtiWarningLetterConditionsService, NtiWarningLetterConditionsService>();
			services.AddScoped<INtiService, NtiService>();
			services.AddScoped<INtiConditionsService, NtiConditionsService>();
			services.AddScoped<ITeamsService, TeamsService>();
			services.AddScoped<IDecisionService, DecisionService>();
			services.AddScoped<ITrustFinancialForecastService, TrustFinancialForecastService>();
			services.AddScoped<ICasePermissionsService, CasePermissionsService>();
			services.AddScoped<IFakeTrustService, FakeTrustService>();
			services.AddScoped<ICityTechnologyCollegeService, CityTechnologyCollegeService>();
			services.AddScoped<ITargetedTrustEngagementService, TargetedTrustEngagementService>();


			// Redis services
			services.AddSingleton<ICacheProvider, CacheProvider>();
			services.AddScoped<IUserStateCachedService, UserStateCachedService>();
			services.AddScoped<ITrustCachedService, TrustCachedService>();
			services.AddScoped<INtiWarningLetterCachedService, NtiWarningLetterCachedService>();
			services.AddScoped<INtiCachedService, NtiCachedService>();
			services.AddScoped<ICaseSummaryService, CaseSummaryService>();

			services.AddScoped<ICorrelationContext, CorrelationContext>();

			services.AddHttpContextAccessor();
			services.AddScoped<IClientUserInfoService, ClientUserInfoService>();
			services.AddSingleton<IPageHistoryStorageHandler, SessionPageHistoryStorageHandler>();
		}

		public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
		{
			var test = configuration.GetSection(TrustSearchOptions.Cache);
			services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache));
			services.Configure<TrustSearchOptions>(configuration.GetSection(TrustSearchOptions.Cache));
			services.Configure<SiteOptions>(configuration.GetSection(SiteOptions.Site));
			services.Configure<FakeTrustOptions>(configuration.GetSection("FakeTrusts"));
			services.Configure<AzureAdGroupsOptions>(configuration.GetSection("AzureAdGroups"));
			services.Configure<AzureAdOptions>(configuration.GetSection("AzureAd"));
		}
	}
}
