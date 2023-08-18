﻿using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome;
using ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;
using ConcernsCaseWork.API.UseCases.Permissions.Cases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
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
			services.AddScoped<ICreateConcernsRecord, CreateConcernsRecord>();
			services.AddScoped<IConcernsTypeGateway, ConcernsTypeGateway>();
			services.AddScoped<IConcernsRatingGateway, ConcernsRatingsGateway>();
			services.AddScoped<IIndexConcernsRatings, IndexConcernsRatings>();
			services.AddScoped<IUpdateConcernsCase, UpdateConcernsCase>();
			services.AddScoped<IDeleteConcernsCase, DeleteConcernsCase>();

			services.AddScoped<IIndexConcernsTypes, IndexConcernsTypes>();
			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();
			services.AddScoped<IDeleteConcernsRecord, DeleteConcernsRecord>();
			services.AddScoped<IGetConcernsRecord, GetConcernsRecord>();

			services.AddScoped<IIndexConcernsMeansOfReferrals, IndexConcernsMeansOfReferrals>();
			services.AddScoped<IConcernsMeansOfReferralGateway, ConcernsMeansOfReferralGateway>();

			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

			services.AddScoped<IGetConcernsRecordsByCaseUrn, GetConcernsRecordsByCaseUrn>();
			services.AddScoped<IGetConcernsCasesByOwnerId, GetConcernsCasesByOwnerId>();
			services.AddScoped<IGetClosedConcernsCaseSummariesByOwner, GetClosedConcernsCaseSummariesByOwner>();
			services.AddScoped<IGetActiveConcernsCaseSummariesByTrust, GetActiveConcernsCaseSummariesByTrust>();
			services.AddScoped<IGetClosedConcernsCaseSummariesByTrust, GetClosedConcernsCaseSummariesByTrust>();
			services.AddScoped<IGetActiveConcernsCaseSummariesForUsersTeam, GetActiveConcernsCaseSummariesForUsersTeam>(); 
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

			services.AddScoped<IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse>, CreateDecisionOutcome>();
			// TODO: Can remove this registration if we use DecisionUseCaseRequestWrapper for all IUseCaseAsync
			services.AddScoped<IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse>, CloseDecision>();
			services.AddScoped<IUseCaseAsync<UpdateDecisionOutcomeUseCaseParams, UpdateDecisionOutcomeResponse>, UpdateDecisionOutcome>();

			// concerns factories
			services.AddScoped<IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse>, CreateDecision>();
			services.AddScoped<IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>, GetDecision>();
			services.AddScoped<IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>, GetDecisions>();
			services.AddScoped<ICreateDecisionResponseFactory, CreateDecisionResponseFactory>();
			services.AddScoped<IDecisionFactory, DecisionFactory>();
			services.AddScoped<IGetDecisionResponseFactory, GetDecisionResponseFactory>();
			services.AddScoped<IGetDecisionsSummariesFactory, GetDecisionsSummariesFactory>();
			services.AddScoped<IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest details), UpdateDecisionResponse>, UpdateDecision>();
			services.AddScoped<IUpdateDecisionResponseFactory, UpdateDecisionResponseFactory>();
			services.AddScoped<ICloseDecisionResponseFactory, CloseDecisionResponseFactory>();
			services.AddScoped<IUseCaseAsync<DeleteDecisionRequest, DeleteDecisionResponse>, DeleteDecision>();


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
			services.AddScoped<IGetCasePermissionsUseCase, GetCasePermissionsUseCase>();
			
			services.AddScoped<ICorrelationContext, CorrelationContext>();

			return services;
		}
	}
}
