using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly IDecisionService _decisionService;
		private readonly ILogger<ClosePageModel> _logger;
		
		public int NotesMaxLength => DecisionConstants.MaxSupportingNotesLength;
		
		[BindProperty(SupportsGet = true)]
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "DecisionId must be provided")]
		public int DecisionId { get; set; }

		[BindProperty(Name="Urn", SupportsGet = true)]
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "CaseUrn must be provided")]
		public int CaseUrn { get; set; }
		
		[BindProperty(Name="SupportingNotes")]
		[MaxLength(DecisionConstants.MaxSupportingNotesLength, ErrorMessage = "Supporting Notes must be 2000 characters or less")]
		public string Notes { get; set; }

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
				if (!ModelState.IsValid)
				{
					return Page();
				}
				
				var decision = await _decisionService.GetDecision(CaseUrn, DecisionId);

				if (!IsDecisionEditable(decision))
				{
					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}
				
				Notes = decision.SupportingNotes;
				
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
					return Page();
				}

				var updateDecisionRequest = new CloseDecisionRequest { SupportingNotes = Notes };

				await _decisionService.CloseDecision(CaseUrn, DecisionId, updateDecisionRequest);

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

		// TODO: These ought to be part of a general service / model
		private static bool IsDecisionEditable(GetDecisionResponse decision) => !IsDecisionClosed(decision) && DecisionHasOutcome(decision);
		private static bool IsDecisionClosed(GetDecisionResponse decision) => decision.ClosedAt.HasValue;
		private static bool DecisionHasOutcome(GetDecisionResponse decision) => decision.Outcome != null;
	}
}