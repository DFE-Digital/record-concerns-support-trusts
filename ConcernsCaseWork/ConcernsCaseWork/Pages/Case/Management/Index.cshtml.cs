using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
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
using Microsoft.Extensions.Primitives;
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
		private readonly IStatusCachedService _statusCachedService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;
		private readonly IActionsModelService _actionsModelService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IUserStateCachedService _cachedService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

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
			IStatusCachedService statusCachedService,
			INtiUnderConsiderationStatusesCachedService ntiUCStatusesCachedService,
			ILogger<IndexPageModel> logger,
			IActionsModelService actionsModelService,
			ICaseSummaryService caseSummaryService,
			ICasePermissionsService casePermissionsService,
			IUserStateCachedService cachedService,
			IClaimsPrincipalHelper claimsPrincipalHelper
			)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_caseModelService = Guard.Against.Null(caseModelService);
			_recordModelService = Guard.Against.Null(recordModelService);
			_ratingModelService = Guard.Against.Null(ratingModelService);
			_statusCachedService = Guard.Against.Null(statusCachedService);
			_ntiStatusesCachedService = Guard.Against.Null(ntiUCStatusesCachedService);
			_logger = Guard.Against.Null(logger);
			_actionsModelService = Guard.Against.Null(actionsModelService);
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
			_casePermissionsService = Guard.Against.Null(casePermissionsService);
			_cachedService = Guard.Against.Null(cachedService);
			_claimsPrincipalHelper = claimsPrincipalHelper;
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
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);

				if (await IsCaseClosed())
				{
					return Redirect($"/case/{CaseModel.Urn}/closed");
				}

				// Check if case is editable
				IsEditableCase = await IsCaseEditable(caseUrn);

				// Map Case Rating
				CaseModel.RatingModel = await _ratingModelService.GetRatingModelById(CaseModel.RatingId);
				// Get Case concerns
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(caseUrn);

				// Map Case concerns
				CaseModel.RecordsModel = recordsModel;

				var trustDetailsTask = _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				var activeTrustCasesTask = _caseSummaryService.GetActiveCaseSummariesByTrust(CaseModel.TrustUkPrn,0,999);
				var closedTrustCasesTask = _caseSummaryService.GetClosedCaseSummariesByTrust(CaseModel.TrustUkPrn,0,999);
				var caseActionsTask = PopulateCaseActions(caseUrn);
				Task.WaitAll(trustDetailsTask, activeTrustCasesTask, closedTrustCasesTask, caseActionsTask);
				TrustDetailsModel = trustDetailsTask.Result;
				ActiveCases = activeTrustCasesTask.Result;
				ClosedCases = closedTrustCasesTask.Result;
				NtiStatuses = (await _ntiStatusesCachedService.GetAllStatuses()).ToList();
				await UpdateCacheService(CaseModel);
				
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
			var caseActionsSummaryBreakdown = await _actionsModelService.GetActionsSummary(caseUrn);

			OpenCaseActions = caseActionsSummaryBreakdown.OpenActions;
			ClosedCaseActions = caseActionsSummaryBreakdown.ClosedActions;
		}

		private async Task<bool> UserHasEditCasePrivileges(long caseId)
		{
			var permissionsResponse = await _casePermissionsService.GetCasePermissions(caseId);
			return permissionsResponse.HasEditPermissions();
		}

		private async Task<bool> IsCaseClosed()
		{
			var closedStatus = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());

			if (CaseModel.StatusId.CompareTo(closedStatus.Id) == 0)
			{
				return true;
			}

			return false;
		}

		private async Task<bool> IsCaseEditable(long caseId)
		{
			var isCaseClosed = await IsCaseClosed();
			var userHasEditCasePrivileges = await UserHasEditCasePrivileges(caseId);

			if (!isCaseClosed && userHasEditCasePrivileges)
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