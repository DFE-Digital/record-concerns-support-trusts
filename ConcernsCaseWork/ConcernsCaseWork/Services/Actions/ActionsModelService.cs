using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Shared;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Actions
{
	public sealed class ActionsModelService : IActionsModelService
	{
		private readonly ISRMAService _srmaService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiUcStatusesCachedService;
		private readonly IDecisionModelService _decisionModelService;
		private readonly ILogger<ActionsModelService> _logger;

		public ActionsModelService(
			ISRMAService srmaService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			ILogger<ActionsModelService> logger, 
			INtiUnderConsiderationStatusesCachedService ntiUcStatusesCachedService,
			IDecisionModelService decisionModelService)
		{
			_srmaService = srmaService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_logger = logger;
			_ntiUcStatusesCachedService = ntiUcStatusesCachedService;
			_decisionModelService = decisionModelService;
		}

		public async Task<ActionSummaryBreakdownModel> GetActionsSummary(long caseUrn)
		{
			var result = new ActionSummaryBreakdownModel();

			try
			{
				var caseActions = new List<ActionSummaryModel>();

				caseActions.AddRange(await GetSrmas(caseUrn));
				caseActions.AddRange(await GetFinancialPlans(caseUrn));
				caseActions.AddRange(await GetNtisUnderConsideration(caseUrn));
				caseActions.AddRange(await GetNtiWarningLettersForCase(caseUrn));
				caseActions.AddRange(await GetNtisForCase(caseUrn));
				caseActions.AddRange(await _decisionModelService.GetDecisionsByUrn(caseUrn));

				result = ActionSummaryMapping.ToActionSummaryBreakdown(caseActions);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{EchoCallerName}::Exception - {Message}", 
					nameof(ActionsModelService), LoggingHelpers.EchoCallerName(), ex.Message);
			}
			
			return result;
		}

		private async Task<IEnumerable<ActionSummaryModel>> GetSrmas(long caseUrn)
			=> (await _srmaService.GetSRMAsForCase(caseUrn))
				.Select(a => a.ToActionSummary());
		
		private async Task<IEnumerable<ActionSummaryModel>> GetFinancialPlans(long caseUrn)
			=> (await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn))

				.Select(a => a.ToActionSummary());

		private async Task<IEnumerable<ActionSummaryModel>> GetNtisUnderConsideration(long caseUrn)
		{
			var statuses = await _ntiUcStatusesCachedService.GetAllStatuses();
			
			return (await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn))
				.Select(a => a.ToActionSummary(statuses));
		}

		private async Task<IEnumerable<ActionSummaryModel>> GetNtiWarningLettersForCase(long caseUrn)
			=> (await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn))
				.Select(a => a.ToActionSummary());
								
		private async Task<IEnumerable<ActionSummaryModel>> GetNtisForCase(long caseUrn)
			=> (await _ntiModelService.GetNtisForCaseAsync(caseUrn))
				.Select(a => a.ToActionSummary());
	}
}