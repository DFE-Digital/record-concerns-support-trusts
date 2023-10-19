using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly IDecisionService _decisionService;
		private readonly ILogger<ClosePageModel> _logger;

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		[BindProperty(SupportsGet = true)]
		public int DecisionId { get; set; }

		[BindProperty(Name="Urn", SupportsGet = true)]
		public int CaseUrn { get; set; }

		public ClosePageModel(IDecisionService decisionService, ILogger<ClosePageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var decision = await _decisionService.GetDecision(CaseUrn, DecisionId);

				if (!IsDecisionEditable(decision))
				{
					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}

				LoadPageComponents(decision);
								
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					LoadPageComponents();
					return Page();
				}

				var updateDecisionRequest = new CloseDecisionRequest { SupportingNotes = Notes.Text.StringContents };

				await _decisionService.CloseDecision(CaseUrn, DecisionId, updateDecisionRequest);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(GetDecisionResponse model)
		{
			LoadPageComponents();

			Notes.Text.StringContents = model.SupportingNotes;
		}

		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent(Notes?.Text.StringContents);
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("SupportingNotes", nameof(Notes), "Finalise supporting notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action. Include any academy name(s) related to the decision",
			Text = new ValidateableString()
			{
				MaxLength = 2000,
				StringContents = contents,
				DisplayName = "Supporting notes"
			}
		};

		private static bool IsDecisionEditable(GetDecisionResponse decision) => !IsDecisionClosed(decision) && DecisionHasOutcome(decision);
		private static bool IsDecisionClosed(GetDecisionResponse decision) => decision.ClosedAt.HasValue;
		private static bool DecisionHasOutcome(GetDecisionResponse decision) => decision.Outcome != null;
	}
}