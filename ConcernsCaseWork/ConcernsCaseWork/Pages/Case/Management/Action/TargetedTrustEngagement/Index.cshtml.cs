using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ITargetedTrustEngagementService _targetedTrustEngagementService;
		private ICasePermissionsService _casePermissionsService;
		private ILogger<IndexPageModel> _logger;

		public ViewTargetedTrustEngagementModel TargetedTrustEngagement { get; set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");
		public bool UserCanDelete { get; private set; }

		public IndexPageModel(
			ITargetedTrustEngagementService targetedTrustEngagementService,
			ICasePermissionsService casePermissionsService,
			ILogger<IndexPageModel> logger)
		{
			_targetedTrustEngagementService = targetedTrustEngagementService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			ViewData[ViewDataConstants.Title] = "Targeted Trust Engagement";

			try
			{
				TryGetRouteValueInt64("urn", out var urn);
				TryGetRouteValueInt64("targetedtrustengagementId", out var targetedtrustengagementId);

				var apiTTE = await _targetedTrustEngagementService.GetTargetedTrustEngagement((int)urn, (int)targetedtrustengagementId);
				var casePermissions = await _casePermissionsService.GetCasePermissions((int)urn);

				TargetedTrustEngagement = TargetedTrustEngagementMapping.ToViewModel(apiTTE, casePermissions);

				UserCanDelete = await _casePermissionsService.UserHasDeletePermissions(urn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				SetErrorMessage(ErrorConstants.ErrorOnGetPage);
			}

			return Page();
		}
	}
}
