using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;
		private readonly IActionsModelService _actionsModelService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IUserStateCachedService _cachedService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ICloseCaseValidatorService _closeCaseValidatorService;

		public readonly string ConcernsErrorKey = "Concerns";
		public readonly string CaseActionsErrorKey = "CaseActions";

		[BindProperty(Name = "Urn", SupportsGet = true)]
		public long CaseUrn { get; set; }

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<ActiveCaseSummaryModel> ActiveCases { get; private set; }
		public IList<ClosedCaseSummaryModel> ClosedCases { get; private set; }
		public List<NtiUnderConsiderationStatusDto> NtiStatuses { get; set; }
		public bool IsEditableCase { get; private set; }

		[TempData]
		public bool CaseOwnerChanged { get; set; }

		public List<ActionSummaryModel> OpenCaseActions { get; set; }
		public List<ActionSummaryModel> ClosedCaseActions { get; set; }

		public IndexPageModel(ICaseModelService caseModelService,
			ITrustModelService trustModelService,
			IRecordModelService recordModelService,
			IRatingModelService ratingModelService,
			INtiUnderConsiderationStatusesCachedService ntiUCStatusesCachedService,
			ILogger<IndexPageModel> logger,
			IActionsModelService actionsModelService,
			ICaseSummaryService caseSummaryService,
			ICasePermissionsService casePermissionsService,
			IUserStateCachedService cachedService,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			ICloseCaseValidatorService closeCaseValidatorService
			)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_caseModelService = Guard.Against.Null(caseModelService);
			_recordModelService = Guard.Against.Null(recordModelService);
			_ratingModelService = Guard.Against.Null(ratingModelService);
			_ntiStatusesCachedService = Guard.Against.Null(ntiUCStatusesCachedService);
			_logger = Guard.Against.Null(logger);
			_actionsModelService = Guard.Against.Null(actionsModelService);
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
			_casePermissionsService = Guard.Against.Null(casePermissionsService);
			_cachedService = Guard.Against.Null(cachedService);
			_claimsPrincipalHelper = claimsPrincipalHelper;
			_closeCaseValidatorService = closeCaseValidatorService;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				// Get Case
				CaseModel = await _caseModelService.GetCaseByUrn(CaseUrn);

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

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var errors = await _closeCaseValidatorService.Validate(CaseUrn);

				if (errors.Count > 0)
				{
					CaseModel = await _caseModelService.GetCaseByUrn(CaseUrn);
					await LoadPage();

					errors.ForEach(error =>
					{
						var key = error.Type == CloseCaseError.Concern ? ConcernsErrorKey : CaseActionsErrorKey;

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

		private async Task LoadPage()
		{
			// Check if case is editable
			IsEditableCase = await IsCaseEditable(CaseUrn);

			// Map Case Rating
			CaseModel.RatingModel = await _ratingModelService.GetRatingModelById(CaseModel.RatingId);
			// Get Case concerns
			var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(CaseUrn);

			// Map Case concerns
			CaseModel.RecordsModel = recordsModel;

			var trustDetailsTask = _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			var activeTrustCasesTask = _caseSummaryService.GetActiveCaseSummariesByTrust(CaseModel.TrustUkPrn, 1);
			var closedTrustCasesTask = _caseSummaryService.GetClosedCaseSummariesByTrust(CaseModel.TrustUkPrn, 1);
			var caseActionsTask = PopulateCaseActions(CaseUrn);
			Task.WaitAll(trustDetailsTask, activeTrustCasesTask, closedTrustCasesTask, caseActionsTask);
			TrustDetailsModel = trustDetailsTask.Result;

			var activeCaseGroup = activeTrustCasesTask.Result;
			var closedCaseGroup = closedTrustCasesTask.Result;

			ActiveCases = activeCaseGroup.Cases;
			ClosedCases = closedCaseGroup.Cases;
			NtiStatuses = (await _ntiStatusesCachedService.GetAllStatuses()).ToList();
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

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
		
		private async Task UpdateCacheService(CaseModel model)
		{
			var userState = await _cachedService.GetData(GetUserName());
			var trustUkPrn = userState.TrustUkPrn;
			if (trustUkPrn == null)
			{
				userState.TrustUkPrn = model.TrustUkPrn;
				await _cachedService.StoreData(userState.UserName, userState);
			}
			
		}
	}
}