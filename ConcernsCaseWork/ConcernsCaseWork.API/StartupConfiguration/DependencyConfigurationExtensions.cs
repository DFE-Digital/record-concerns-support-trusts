using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.API.Features.ConcernsRating;
using ConcernsCaseWork.API.Features.ConcernsStatus;
using ConcernsCaseWork.API.Features.ConcernsType;
using ConcernsCaseWork.API.Features.MeansOfReferral;
using ConcernsCaseWork.API.Features.Permissions;
using ConcernsCaseWork.API.Features.TeamCasework;
using ConcernsCaseWork.API.Features.TrustFinancialForecast;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.StartupConfiguration
{
	public static class DependencyConfigurationExtensions
	{
		public static IServiceCollection AddUseCases(this IServiceCollection services)
		{
			var allTypes = typeof(IUseCase<,>).Assembly.GetTypes();

			foreach (var type in allTypes)
			{
				foreach (var @interface in type.GetInterfaces())
				{
					if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IUseCase<,>))
					{
						services.AddScoped(@interface, type);
					}
				}
			}

			return services;
		}

		public static IServiceCollection AddUseCaseAsyncs(this IServiceCollection services)
		{
			var allTypes = typeof(IUseCaseAsync<,>).Assembly.GetTypes();

			foreach (var type in allTypes)
			{
				foreach (var @interface in type.GetInterfaces())
				{
					if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IUseCaseAsync<,>))
					{
						services.AddScoped(@interface, type);
					}
				}
			}

			return services;
		}

		public static IServiceCollection AddApiDependencies(this IServiceCollection services)
		{
			services.AddScoped<ICreateConcernsCase, CreateConcernsCase>();
			services.AddScoped<IConcernsCaseGateway, ConcernsCaseGateway>();
			services.AddScoped<IGetConcernsCaseByUrn, GetConcernsCaseByUrn>();
			services.AddScoped<IGetConcernsCaseByTrustUkprn, GetConcernsCaseByTrustUkprn>();
			services.AddScoped<IIndexConcernsStatuses, IndexConcernsStatuses>();
			services.AddScoped<IConcernsStatusGateway, ConcernsStatusGateway>();
			services.AddScoped<IConcernsRecordGateway, ConcernsRecordGateway>();
			services.AddScoped<IConcernsTypeGateway, ConcernsTypeGateway>();
			services.AddScoped<IConcernsRatingGateway, ConcernsRatingsGateway>();
			services.AddScoped<IIndexConcernsRatings, IndexConcernsRatings>();
			services.AddScoped<IUpdateConcernsCase, UpdateConcernsCase>();
			services.AddScoped<IDeleteConcernsCase, DeleteConcernsCase>();

			services.AddScoped<IIndexConcernsTypes, IndexConcernsTypes>();

			services.AddScoped<IIndexConcernsMeansOfReferrals, IndexConcernsMeansOfReferrals>();
			services.AddScoped<IConcernsMeansOfReferralGateway, ConcernsMeansOfReferralGateway>();

			services.AddScoped<IGetCaseFilterParameters, GetCaseFilterParameters>();
			services.AddScoped<IGetConcernsCasesByOwnerId, GetConcernsCasesByOwnerId>();
			services.AddScoped<IGetClosedConcernsCaseSummariesByOwner, GetClosedConcernsCaseSummariesByOwner>();
			services.AddScoped<IGetActiveConcernsCaseSummariesByTrust, GetActiveConcernsCaseSummariesByTrust>();
			services.AddScoped<IGetClosedConcernsCaseSummariesByTrust, GetClosedConcernsCaseSummariesByTrust>();
			services.AddScoped<IGetActiveConcernsCaseSummariesForUsersTeam, GetActiveConcernsCaseSummariesForUsersTeam>(); 
			services.AddScoped<IGetConcernsCaseSummariesByFilter, GetConcernsCaseSummariesByFilter>(); 
			services.AddScoped<IGetActiveConcernsCaseSummariesByOwner, GetActiveConcernsCaseSummariesByOwner>(); 

			services.AddScoped<ISRMAGateway, SRMAGateway>();
			services.AddScoped<IFinancialPlanGateway, FinancialPlanGateway>();
			services.AddScoped<INTIUnderConsiderationGateway, NTIUnderConsiderationGateway>();
			services.AddScoped<INTIWarningLetterGateway, NTIWarningLetterGateway>();
			services.AddScoped<INoticeToImproveGateway, NoticeToImproveGateway>();
			services.AddScoped<IDecisionOutcomeGateway, DecisionOutcomeGateway>();

			services.AddScoped<IGetConcernsCaseworkTeam, GetConcernsCaseworkTeam>();
			services.AddScoped<IGetConcernsCaseworkTeamOwners, GetConcernsCaseworkTeamOwners>();
			services.AddScoped<IUpdateConcernsCaseworkTeam, UpdateConcernsCaseworkTeam>();
			services.AddScoped<IConcernsTeamCaseworkGateway, ConcernsTeamCaseworkGateway>();
			services.AddScoped<IGetOwnersOfOpenCases, GetOwnersOfOpenCases>();
			
			services.AddScoped<ICaseSummaryGateway, CaseSummaryGateway>();


			services.AddScoped<IUseCaseAsync<CreateTrustFinancialForecastRequest, int>, CreateTrustFinancialForecast>();
			services.AddScoped<IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>, UpdateTrustFinancialForecast>();
			services.AddScoped<IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse>, GetTrustFinancialForecastById>();
			services.AddScoped<IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>, GetTrustFinancialForecastsForCase>();
			services.AddScoped<IUseCaseAsync<CloseTrustFinancialForecastRequest, int>, CloseTrustFinancialForecast>();
			services.AddScoped<IUseCaseAsync<DeleteTrustFinancialForecastRequest, int>, DeleteTrustFinancialForecast>();
			services.AddScoped<ITrustFinancialForecastGateway, TrustFinancialForecastGateway>();

			// case action permission strategies
			services.AddScoped<IServerUserInfoService, ServerUserInfoService>();
			services.AddScoped<ICaseActionPermissionStrategyRoot, CaseActionPermissionStrategyRoot>();
			services.AddScoped<ICaseActionPermissionStrategy, IsCaseViewableStrategy>();
			services.AddScoped<ICaseActionPermissionStrategy, IsCaseEditableStrategy>();
			services.AddScoped<ICaseActionPermissionStrategy, IsCaseDeletableStrategy>();

			services.AddScoped<IGetCasePermissionsUseCase, GetCasePermissionsUseCase>();
			
			services.AddScoped<ICorrelationContext, CorrelationContext>();

			return services;
		}
	}
}
