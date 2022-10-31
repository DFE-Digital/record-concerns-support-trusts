﻿using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.Status;
using Service.TRAMS.NtiUnderConsideration;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ICaseHistoryModelService _caseHistoryModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ISRMAService _srmaService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly IDecisionModelService _decisionModelService;
		private readonly ILogger<IndexPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<TrustCasesModel> TrustCasesModel { get; private set; }
		public IList<CaseHistoryModel> CasesHistoryModel { get; private set; }
		public bool IsEditableCase { get; private set; }
		public bool HasOpenActions { get { return CaseActions.Any(a => !a.ClosedAt.HasValue); } }
		public bool HasClosedActions { get { return CaseActions.Any(a => a.ClosedAt.HasValue); } }
		public List<CaseActionModel> CaseActions { get; private set; }
		public List<NtiUnderConsiderationStatusDto> NtiStatuses { get; set; }


		public IndexPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ICaseHistoryModelService caseHistoryModelService,
			IRecordModelService recordModelService,
			IRatingModelService ratingModelService,
			IStatusCachedService statusCachedService,
			ISRMAService srmaService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiUnderConsiderationStatusesCachedService ntiUCStatusesCachedService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			ILogger<IndexPageModel> logger,
			IDecisionModelService decisionModelService
			)
		{
			_caseHistoryModelService = caseHistoryModelService;
			_trustModelService = trustModelService;
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_statusCachedService = statusCachedService;
			_srmaService = srmaService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiStatusesCachedService = ntiUCStatusesCachedService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_logger = logger;
			_decisionModelService = decisionModelService;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ManagementPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				// Get Case
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);

				if (await IsCaseClosed())
				{
					return Redirect($"/case/{CaseModel.Urn}/closed");
				}

				// Check if case is editable
				IsEditableCase = await IsCaseEditable();
				
				// Map Case Rating
				CaseModel.RatingModel = await _ratingModelService.GetRatingModelByUrn(CaseModel.RatingUrn);
				
				// Get Case concerns
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
				
				// Map Case concerns
				CaseModel.RecordsModel = recordsModel;

				var caseHistoryTask = _caseHistoryModelService.GetCasesHistory(User.Identity.Name, caseUrn);
				var trustDetailsTask = _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				var trustCasesTask = _caseModelService.GetCasesByTrustUkprn(CaseModel.TrustUkPrn);
				var caseActionsTask = PopulateCaseActions(caseUrn);
				
				Task.WaitAll(caseHistoryTask, trustDetailsTask, trustCasesTask, caseActionsTask);

				CasesHistoryModel = caseHistoryTask.Result;
				TrustDetailsModel = trustDetailsTask.Result;
				TrustCasesModel = trustCasesTask.Result;
				
				NtiStatuses = (await _ntiStatusesCachedService.GetAllStatuses()).ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ManagementPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		private async Task PopulateCaseActions(long caseUrn)
		{
			CaseActions = CaseActions ?? new List<CaseActionModel>();
			CaseActions.AddRange(await _srmaService.GetSRMAsForCase(caseUrn));
			CaseActions.AddRange(await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, User.Identity.Name));
			CaseActions.AddRange(await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn));
			CaseActions.AddRange(await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn));
			CaseActions.AddRange(await _ntiModelService.GetNtisForCaseAsync(caseUrn));
			CaseActions.AddRange(await _decisionModelService.GetDecisionsByUrn(caseUrn));
		}

		private bool UserHasEditCasePrivileges()
		{
			bool result = CaseModel.CreatedBy.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase);
			return result;
		}

		private async Task<bool> IsCaseClosed()
		{
			var closedStatus = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());

			if (CaseModel.StatusUrn.CompareTo(closedStatus.Urn) == 0)
			{
				return true;
			}

			return false;
		}

		private async Task<bool> IsCaseEditable()
		{
			var isCaseClosed = await IsCaseClosed();
			var userHasEditCasePrivileges = UserHasEditCasePrivileges();

			if (!isCaseClosed && userHasEditCasePrivileges)
			{
				return true;
			}

			return false;
		}
	}
}