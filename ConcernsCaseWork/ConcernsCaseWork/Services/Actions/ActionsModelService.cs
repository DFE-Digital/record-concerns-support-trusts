using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.Services.Cases;
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
		private readonly ILogger<ActionsModelService> _logger;

		public ActionsModelService(
			ISRMAService srmaService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			ILogger<ActionsModelService> logger, INtiUnderConsiderationStatusesCachedService ntiUcStatusesCachedService)
		{
			_srmaService = srmaService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_logger = logger;
			_ntiUcStatusesCachedService = ntiUcStatusesCachedService;
		}

		public async Task<IList<ActionSummary>> GetClosedActionsSummary(string userName, long caseUrn)
		{
			var caseActions = new List<ActionSummary>();
				
			try
			{
				caseActions.AddRange(await GetClosedSrmas(caseUrn));
				caseActions.AddRange(await GetClosedFinancialPlans(caseUrn, userName));
				caseActions.AddRange(await GetClosedNtisUnderConsideration(caseUrn));
				caseActions.AddRange(await GetClosedNtiWarningLettersForCase(caseUrn));
				caseActions.AddRange(await GetClosedNtisForCase(caseUrn));
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{EchoCallerName}::Exception - {Message}", 
					nameof(ActionsModelService), LoggingHelpers.EchoCallerName(), ex.Message);
			}
			
			return caseActions;
		}

		private async Task<IEnumerable<ActionSummary>> GetClosedSrmas(long caseUrn)
			=> (await _srmaService.GetSRMAsForCase(caseUrn))
				.Where(a => a.ClosedAt.HasValue)
				.Select(a => a.ToActionSummary());
		
		private async Task<IEnumerable<ActionSummary>> GetClosedFinancialPlans(long caseUrn, string userName)
			=> (await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, userName))
				.Where(a => a.ClosedAt.HasValue)
				.Select(a => a.ToActionSummary());

		private async Task<IEnumerable<ActionSummary>> GetClosedNtisUnderConsideration(long caseUrn)
		{
			var statuses = await _ntiUcStatusesCachedService.GetAllStatuses();
			
			return (await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn))
				.Where(a => a.ClosedAt.HasValue)
				.Select(a => a.ToActionSummary(statuses));
		}

		private async Task<IEnumerable<ActionSummary>> GetClosedNtiWarningLettersForCase(long caseUrn)
			=> (await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn))
				.Where(a => a.ClosedAt.HasValue)
				.Select(a => a.ToActionSummary());
								
		private async Task<IEnumerable<ActionSummary>> GetClosedNtisForCase(long caseUrn)
			=> (await _ntiModelService.GetNtisForCaseAsync(caseUrn))
				.Where(a => a.ClosedAt.HasValue)
				.Select(a => a.ToActionSummary());
	}
}