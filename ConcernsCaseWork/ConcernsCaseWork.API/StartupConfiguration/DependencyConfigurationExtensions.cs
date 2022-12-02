﻿using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Gateways;
using System.Net.Mime;

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

		public static IServiceCollection AddDependencies(this IServiceCollection services)
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
			services.AddScoped<IIndexConcernsTypes, IndexConcernsTypes>();
			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

			services.AddScoped<IIndexConcernsMeansOfReferrals, IndexConcernsMeansOfReferrals>();
			services.AddScoped<IConcernsMeansOfReferralGateway, ConcernsMeansOfReferralGateway>();

			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

			services.AddScoped<IGetConcernsRecordsByCaseUrn, GetConcernsRecordsByCaseUrn>();
			services.AddScoped<IGetConcernsCasesByOwnerId, GetConcernsCasesByOwnerId>();

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

			services.AddScoped<IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse>, CreateDecisionOutcome>();
			// TODO: Can remove this registration if we use DecisionUseCaseRequestWrapper for all IUseCaseAsync
			services.AddScoped<IUseCaseAsync<DecisionUseCaseRequestWrapper<CloseDecisionRequest>, CloseDecisionResponse>, CloseDecision>();
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

			return services;
		}
	}
}
