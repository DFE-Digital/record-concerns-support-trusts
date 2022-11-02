﻿using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Shared;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.Extensions.Logging;
using Service.Redis.NtiUnderConsideration;
using Service.TRAMS.Helpers;
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

		public async Task<IList<ActionSummary>> GetActionsSummary(string userName, long caseUrn)
		{
			var caseActions = new List<ActionSummary>();
				
			try
			{
				caseActions.AddRange(await GetSrmas(caseUrn));
				caseActions.AddRange(await GetFinancialPlans(caseUrn, userName));
				caseActions.AddRange(await GetNtisUnderConsideration(caseUrn));
				caseActions.AddRange(await GetNtiWarningLettersForCase(caseUrn));
				caseActions.AddRange(await GetNtisForCase(caseUrn));
				caseActions.AddRange(await _decisionModelService.GetDecisionsByUrn(caseUrn));

				return caseActions;
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{EchoCallerName}::Exception - {Message}", 
					nameof(ActionsModelService), LoggingHelpers.EchoCallerName(), ex.Message);
			}
			
			return caseActions;
		}

		private async Task<IEnumerable<ActionSummary>> GetSrmas(long caseUrn)
			=> (await _srmaService.GetSRMAsForCase(caseUrn))
				.Select(a => a.ToActionSummary());
		
		private async Task<IEnumerable<ActionSummary>> GetFinancialPlans(long caseUrn, string userName)
			=> (await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, userName))

				.Select(a => a.ToActionSummary());

		private async Task<IEnumerable<ActionSummary>> GetNtisUnderConsideration(long caseUrn)
		{
			var statuses = await _ntiUcStatusesCachedService.GetAllStatuses();
			
			return (await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn))
				.Select(a => a.ToActionSummary(statuses));
		}

		private async Task<IEnumerable<ActionSummary>> GetNtiWarningLettersForCase(long caseUrn)
			=> (await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn))
				.Select(a => a.ToActionSummary());
								
		private async Task<IEnumerable<ActionSummary>> GetNtisForCase(long caseUrn)
			=> (await _ntiModelService.GetNtisForCaseAsync(caseUrn))
				.Select(a => a.ToActionSummary());
	}
}