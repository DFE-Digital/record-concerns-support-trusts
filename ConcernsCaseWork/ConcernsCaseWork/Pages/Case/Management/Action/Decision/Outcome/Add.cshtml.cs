
using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
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

		public long? DecisionId { get; set; }

		public long CaseUrn { get; set; }

		public List<BusinessArea> BusinessAreaCheckBoxes => Enum.GetValues<BusinessArea>().ToList();
		public List<DecisionOutcome> DecisionOutcomesCheckBoxes => Enum.GetValues<DecisionOutcome>().ToList();
		public List<Authoriser> AuthoriserCheckBoxes => Enum.GetValues<Authoriser>().ToList();

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId);

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

		public async Task<IActionResult> OnPostAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId);

				if (!ModelState.IsValid)
				{
					TempData["Decision.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return Page();
				}

				DecisionOutcome.DecisionMadeDate = ParseDate(Request.Form["dtr-day-decision-made"], Request.Form["dtr-month-decision-made"], Request.Form["dtr-year-decision-made"]);
				DecisionOutcome.DecisionTakeEffectDate = ParseDate(Request.Form["dtr-day-take-effect"], Request.Form["dtr-month-take-effect"], Request.Form["dtr-year-take-effect"]);

				if (decisionId.HasValue)
				{
					//var updateDecisionRequest = DecisionMapping.ToUpdateDecision(Decision);
					//await _decisionService.PutDecision(urn, (long)decisionId, updateDecisionRequest);

					//return Redirect($"/case/{urn}/management/action/decision/{decisionId}");
				}

				//await _decisionService.PostDecision(Decision);

				return Redirect($"/case/{urn}/management");
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

		private void SetupPage(long caseUrn, long? decisionId)
		{
			ViewData[ViewDataConstants.Title] = decisionId.HasValue ? "Edit decision" : "Add decision";

			CaseUrn = (CaseUrn)caseUrn;
			DecisionId = decisionId;
		}

		private async Task<CreateDecisionOutcomeRequest> CreateDecisionOutcomeModel(long caseUrn, long? decisionId)
		{
			/*
			var result = new CreateDecisionRequest();

			result.ConcernsCaseUrn = (int)caseUrn;

			if (decisionId.HasValue)
			{
				var apiDecision = await _decisionService.GetDecision(caseUrn, (int)decisionId);

				result = DecisionMapping.ToEditDecisionModel(apiDecision);
			}

			return result;

			*/


			return new CreateDecisionOutcomeRequest()
			{
				DecisionOutcome = API.Contracts.Enums.DecisionOutcome.PartiallyApproved,
				TotalAmountApproved = 233232,
				DecisionMadeDate = DateTimeOffset.Now,
				DecisionTakeEffectDate = DateTimeOffset.Now,
				Authoriser = Authoriser.G7,
				BusinessAreasConsulted = new BusinessArea[] { BusinessArea.BusinessPartner, BusinessArea.Funding },
				DecisionId = 7
			};
		}


		private DateTime ParseDate(string day, string month, string year)
		{
			var dtString = $"{day}-{month}-{year}";
			var isValidDate = DateTimeHelper.TryParseExact(dtString, out DateTime result);

			if (dtString != "--" && !isValidDate)
			{
				throw new InvalidUserInputException($"{dtString} is an invalid date");
			}

			return result;
		}

	}
}