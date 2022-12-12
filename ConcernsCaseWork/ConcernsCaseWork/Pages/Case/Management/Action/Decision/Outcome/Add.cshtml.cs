
using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
#nullable disable

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision.Outcome
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly IDecisionService _decisionService;
		private readonly ILogger<AddPageModel> _logger;

		[BindProperty]
		public EditDecisionOutcomeModel DecisionOutcome { get; set; }


		public long CaseUrn { get; set; }

		public long DecisionId { get; set; }

		public long? OutcomeId { get; set; }

		public List<DecisionOutcomeBusinessArea> BusinessAreaCheckBoxes => Enum.GetValues<DecisionOutcomeBusinessArea>().ToList();
		public List<DecisionOutcomeStatus> DecisionOutcomesCheckBoxes => Enum.GetValues<DecisionOutcomeStatus>().ToList();
		public List<DecisionOutcomeAuthorizer> AuthoriserCheckBoxes => Enum.GetValues<DecisionOutcomeAuthorizer>().ToList();

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync(long urn, long decisionId, long? outcomeId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId, outcomeId);

				DecisionOutcome = await CreateDecisionOutcomeModel(urn, decisionId);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(long urn, long decisionId, long? outcomeId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId, outcomeId);

				if (!ModelState.IsValid)
				{
					TempData["Decision.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return Page();
				}

				if (OutcomeId.HasValue)
				{
					var updateDecisionOutcomeRequest = DecisionMapping.ToUpdateDecisionOutcome(DecisionOutcome);
					await _decisionService.PutDecisionOutcome(urn, decisionId, updateDecisionOutcomeRequest);

					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}

				var request = DecisionMapping.ToCreateDecisionOutcomeRequest(DecisionOutcome);

				await _decisionService.PostDecisionOutcome(urn, decisionId, request);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (InvalidUserInputException ex)
			{
				TempData["Decision.Message"] = new List<string>() { ex.Message };
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}

		private void SetupPage(long urn, long decisionId, long? outcomeId)
		{
			ViewData[ViewDataConstants.Title] = outcomeId.HasValue ? "Edit outcome" : "Add outcome";

			CaseUrn = (CaseUrn)urn;
			DecisionId = decisionId;
			OutcomeId = outcomeId;
		}

		private async Task<EditDecisionOutcomeModel> CreateDecisionOutcomeModel(long urn, long decisionId)
		{
			var result = new EditDecisionOutcomeModel();

			var apiDecision = await _decisionService.GetDecision(urn, (int)decisionId);
			result = DecisionMapping.ToEditDecisionOutcomeModel(apiDecision);

			return result;
		}
	}
}