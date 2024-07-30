using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	public class IndexPageModel : AbstractPageModel
	{
		private IDecisionService _decisionService;
		private ICaseModelService _caseModelService;
		private ICasePermissionsService _casePermissionsService;
		private ILogger<IndexPageModel> _logger;

		public ViewDecisionModel Decision { get; set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");

		public Division? Division { get; set; }

		public bool UserCanDelete { get; private set; }

		public IndexPageModel(
			IDecisionService decisionService,
			ICaseModelService caseModelService,
			ICasePermissionsService casePermissionsService,
			ILogger<IndexPageModel> logger)
		{
			_decisionService = decisionService;
			_caseModelService = caseModelService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			ViewData[ViewDataConstants.Title] = "Decision";

			try
			{
				TryGetRouteValueInt64("urn", out var urn);
				TryGetRouteValueInt64("decisionId", out var decisionId);

				var apiDecision = await _decisionService.GetDecision(urn, (int)decisionId);
				var casePermissions = await _casePermissionsService.GetCasePermissions((int)urn);
				var caseModelService = await _caseModelService.GetCaseByUrn(urn);

				Division = caseModelService.Division;

				Decision = DecisionMapping.ToViewDecisionModel(apiDecision, casePermissions);

				UserCanDelete = await _casePermissionsService.UserHasDeletePermissions(urn);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::Decision::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				SetErrorMessage(ErrorConstants.ErrorOnGetPage);
			}

			return Page();
		}
	}
}
