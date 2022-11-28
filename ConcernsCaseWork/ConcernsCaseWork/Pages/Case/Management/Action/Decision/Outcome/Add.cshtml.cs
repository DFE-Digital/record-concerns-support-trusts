
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
		public CreateDecisionOutcomeRequest DecisionOutcome { get; set; }

		[BindProperty]
		public OptionalDateModel DecisionMadeDate { get; set; }

		[BindProperty]
		public OptionalDateModel DecisionTakeEffectDate { get; set; }

		public long CaseUrn { get; set; }

		public long DecisionId { get; set; }

		public long? OutcomeId { get; set; }

		public List<BusinessArea> BusinessAreaCheckBoxes => Enum.GetValues<BusinessArea>().ToList();
		public List<DecisionOutcome> DecisionOutcomesCheckBoxes => Enum.GetValues<DecisionOutcome>().ToList();
		public List<Authoriser> AuthoriserCheckBoxes => Enum.GetValues<Authoriser>().ToList();

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

				DecisionOutcome = await CreateDecisionOutcomeModel(decisionId, outcomeId);

				DecisionMadeDate = new OptionalDateModel()
				{
					Day = DecisionOutcome.DecisionMadeDate?.Day.ToString("00"),
					Month = DecisionOutcome.DecisionMadeDate?.Month.ToString("00"),
					Year = DecisionOutcome.DecisionMadeDate?.Year.ToString()
				};

				DecisionTakeEffectDate = new OptionalDateModel()
				{
					Day = DecisionOutcome.DecisionTakeEffectDate?.Day.ToString("00"),
					Month = DecisionOutcome.DecisionTakeEffectDate?.Month.ToString("00"),
					Year = DecisionOutcome.DecisionTakeEffectDate?.Year.ToString()
				};

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

				DecisionOutcome.DecisionMadeDate = ParseDate(DecisionMadeDate);
				DecisionOutcome.DecisionTakeEffectDate = ParseDate(DecisionTakeEffectDate);

				if (OutcomeId.HasValue)
				{
					//var updateDecisionRequest = DecisionMapping.ToUpdateDecision(Decision);
					//await _decisionService.PutDecision(urn, (long)decisionId, updateDecisionRequest);

					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}

				await _decisionService.PostDecisionOutcome(DecisionOutcome);

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

		private async Task<CreateDecisionOutcomeRequest> CreateDecisionOutcomeModel(long decisionId, long? outcomeId)
		{
			var result = new CreateDecisionOutcomeRequest();
			result.DecisionId = decisionId;

			if (outcomeId.HasValue)
			{
				var apiDecisionOutcome = await _decisionService.GetDecisionOutcome(decisionId, outcomeId.Value);
				result = DecisionMapping.ToEditDecisionModel(apiDecisionOutcome);
			}

			return result;
		}


		private DateTime ParseDate(OptionalDateModel date)
		{
			if (date.IsEmpty())
			{
				return new DateTime();
			}

			var result = DateTimeHelper.ParseExact(date.ToString());

			return result;
		}

	}
}