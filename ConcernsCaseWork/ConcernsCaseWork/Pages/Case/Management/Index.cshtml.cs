using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly IRecordModelService _recordModelService;
		private readonly IActionsModelService _actionsModelService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IUserStateCachedService _cachedService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ICaseValidatorService _caseValidatorService;

		public readonly string ConcernsErrorKey = "Concerns";
		public readonly string CaseActionsErrorKey = "CaseActions";

		[BindProperty(Name = "Urn", SupportsGet = true)]
		public long CaseUrn { get; set; }

		public CaseModel CaseModel { get; private set; }

		//public string RatingRationalCommentary { get; set; }

		public TrustOverviewModel TrustOverviewModel { get; set; }

		public bool IsEditableCase { get; private set; }
		public bool UserCanDelete { get; private set; }

	[TempData]
		public bool CaseOwnerChanged { get; set; }

		public List<ActionSummaryModel> OpenCaseActions { get; set; }
		public List<ActionSummaryModel> ClosedCaseActions { get; set; }

		public IndexPageModel(ICaseModelService caseModelService,
			ITrustModelService trustModelService,
			IRecordModelService recordModelService,
			ILogger<IndexPageModel> logger,
			IActionsModelService actionsModelService,
			ICaseSummaryService caseSummaryService,
			ICasePermissionsService casePermissionsService,
			IUserStateCachedService cachedService,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			ICaseValidatorService caseValidatorService
			)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_caseModelService = Guard.Against.Null(caseModelService);
			_recordModelService = Guard.Against.Null(recordModelService);
			_logger = Guard.Against.Null(logger);
			_actionsModelService = Guard.Against.Null(actionsModelService);
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
			_casePermissionsService = Guard.Against.Null(casePermissionsService);
			_cachedService = Guard.Against.Null(cachedService);
			_claimsPrincipalHelper = claimsPrincipalHelper;
			_caseValidatorService = caseValidatorService;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				// Get Case
				CaseModel = await _caseModelService.GetCaseByUrn(CaseUrn);

				//RatingRationalCommentary = CaseModel.RatingRationalCommentary;

				if (CaseModel.IsClosed())
				{
					return Redirect($"/case/{CaseModel.Urn}/closed");
				}

				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostCloseCaseAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var errors = await _caseValidatorService.ValidateClose(CaseUrn);

				if (errors.Count > 0)
				{
					CaseModel = await _caseModelService.GetCaseByUrn(CaseUrn);
					await LoadPage();

					errors.ForEach(error =>
					{
						var key = error.Type == CaseValidationError.Concern ? ConcernsErrorKey : CaseActionsErrorKey;

						ModelState.AddModelError(key, error.Error);
					});

					return Page();
				}

				return Redirect($"/case/{CaseUrn}/management/closure");
			}
			catch(Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
		
		public async Task<IActionResult> OnPostDeleteCaseAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var errors = await _caseValidatorService.ValidateDelete(CaseUrn);

				if (errors.Count > 0)
				{
					CaseModel = await _caseModelService.GetCaseByUrn(CaseUrn);
					await LoadPage();

					errors.ForEach(error =>
					{
						var key = error.Type == CaseValidationError.Concern ? ConcernsErrorKey : CaseActionsErrorKey;

						ModelState.AddModelError(key, error.Error);
					});

					return Page();
				}

				return Redirect($"/case/{CaseUrn}/management/delete");
			}
			catch(Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private async Task LoadPage()
		{
			// Check if case is editable
			IsEditableCase = await IsCaseEditable(CaseUrn);


			// Check if case is editable
			UserCanDelete = await IsCaseDeletable(CaseUrn);

			// Get Case concerns
			var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(CaseUrn);

			// Map Case concerns
			CaseModel.RecordsModel = recordsModel;

			var trustDetailsTask = _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			var activeTrustCasesTask = GetActiveCases(CaseModel.TrustUkPrn, 1);
			var closedTrustCasesTask = GetClosedCases(CaseModel.TrustUkPrn, 1);
			var caseActionsTask = PopulateCaseActions(CaseUrn);
			Task.WaitAll(trustDetailsTask, activeTrustCasesTask, closedTrustCasesTask, caseActionsTask);

			TrustOverviewModel = new TrustOverviewModel()
			{
				ActiveCaseSummaryGroupModel = activeTrustCasesTask.Result,
				ClosedCaseSummaryGroupModel = closedTrustCasesTask.Result,
				TrustDetailsModel = trustDetailsTask.Result
			};
			
			await UpdateCacheService(CaseModel);
		}

		private async Task PopulateCaseActions(long caseUrn)
		{
			var caseActionsSummaryBreakdown = await _actionsModelService.GetActionsSummary(caseUrn);

			OpenCaseActions = caseActionsSummaryBreakdown.OpenActions;
			ClosedCaseActions = caseActionsSummaryBreakdown.ClosedActions;
		}

		private async Task<bool> UserHasEditCasePrivileges(long caseId)
		{
			var permissionsResponse = await _casePermissionsService.GetCasePermissions(caseId);
			return permissionsResponse.HasEditPermissions();
		}

		private async Task<bool> IsCaseEditable(long caseId)
		{
			var userHasEditCasePrivileges = await UserHasEditCasePrivileges(caseId);

			if (!CaseModel.IsClosed() && userHasEditCasePrivileges)
			{
				return true;
			}

			return false;
		}
		
		private async Task<bool> IsCaseDeletable(long caseId)
		{
			var userHasDeletePrivileges = await _casePermissionsService.UserHasDeletePermissions(caseId);

			if (!CaseModel.IsClosed() && userHasDeletePrivileges)
			{
				return true;
			}

			return false;
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
		
		private async Task UpdateCacheService(CaseModel model)
		{
			var userName = GetUserName();

			var userState = await _cachedService.GetData(userName);

			if (userState?.TrustUkPrn != null)
			{
				return;
			}

			if (userState != null)
					userState.TrustUkPrn = model.TrustUkPrn;
				await _cachedService.StoreData(userName, userState);
		}

		public async Task<IActionResult> OnGetPaginatedActiveCases(string trustUkPrn, int pageNumber)
		{
			var activeCaseSummaryGroup = await GetActiveCases(trustUkPrn, pageNumber);

			return Partial("_TrustActiveCases", activeCaseSummaryGroup);
		}

		public async Task<IActionResult> OnGetPaginatedClosedCases(string trustUkPrn, int pageNumber)
		{
			var closedCaseSummaryGroup = await GetClosedCases(trustUkPrn, pageNumber);

			return Partial("_TrustClosedCases", closedCaseSummaryGroup);
		}

		private async Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCases(string trustUkPrn, int pageNumber)
		{
			var result = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkPrn, pageNumber);
			result.Pagination.Url = $"/case/{CaseUrn}/management?handler=PaginatedActiveCases&trustUkPrn={trustUkPrn}";
			result.Pagination.ContentContainerId = "active-cases";
			result.Pagination.ElementIdPrefix = "active-cases-";

			return result;
		}

		private async Task<CaseSummaryGroupModel<ClosedCaseSummaryModel>> GetClosedCases(string trustUkPrn, int pageNumber)
		{
			var result = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkPrn, pageNumber);
			result.Pagination.Url = $"/case/{CaseUrn}/management?handler=PaginatedClosedCases&trustUkPrn={trustUkPrn}";
			result.Pagination.ContentContainerId = "closed-cases";
			result.Pagination.ElementIdPrefix = "closed-cases-";

			return result;
		}
	}
}