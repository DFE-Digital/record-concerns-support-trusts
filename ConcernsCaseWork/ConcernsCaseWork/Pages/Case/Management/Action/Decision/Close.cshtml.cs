using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
		public long DecisionId { get; set; }

		[BindProperty(Name="Urn", SupportsGet = true)]
		[Required]
		public long CaseUrn { get; set; }
		
		[BindProperty]
		[MaxLength(DecisionConstants.MaxSupportingNotesLength)]
		public string Notes { get; set; }

		public ClosePageModel(IDecisionService decisionService, ILogger<ClosePageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public IActionResult OnGetAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
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

		private async Task<CreateDecisionRequest> CreateDecisionModel(long caseUrn, long? decisionId)
		{
			var result = new CreateDecisionRequest();

			result.ConcernsCaseUrn = (int)caseUrn;

			if (decisionId.HasValue)
			{
				var apiDecision = await _decisionService.GetDecision(caseUrn, (int)decisionId);

				result = DecisionMapping.ToEditDecisionModel(apiDecision);
			}

			return result;
		}
	}
}