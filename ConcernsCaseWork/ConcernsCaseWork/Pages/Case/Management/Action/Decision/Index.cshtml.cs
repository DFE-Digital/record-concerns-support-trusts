using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
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
		private ILogger _logger;

		public DecisionModel Decision { get; set; }

		public IndexPageModel(
			IDecisionService decisionService,
			ILogger<IndexPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				TryGetRouteValueInt64("urn", out var urn);
				TryGetRouteValueInt64("decisionId", out var decisionId);

				var decision = await _decisionService.GetDecision(urn, (int)decisionId);

				Decision = DecisionMapping.ToDecisionModel(decision);
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
