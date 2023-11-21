using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Utils.Extensions;

#nullable disable

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision.Outcome
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly IDecisionService _decisionService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<AddPageModel> _logger;

		[BindProperty]
		public EditDecisionOutcomeModel DecisionOutcome { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent DecisionOutcomeStatus { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DecisionMadeDate { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DecisionEffectiveFromDate { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent DecisionOutcomeAuthorizer { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public long CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "decisionId")]
		public long DecisionId { get; set; }

		[BindProperty(SupportsGet = true, Name = "outcomeId")]
		public long? OutcomeId { get; set; }

		[BindProperty]
		public Division? Division { get; set; }

		public string SaveAndContinueButtonText { get; set; }

		public List<DecisionOutcomeBusinessArea> BusinessAreaCheckBoxes { get; set; }

		public AddPageModel(
			IDecisionService decisionService,
			ICaseModelService caseModelService,
			ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_caseModelService = caseModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				DecisionOutcome = await CreateDecisionOutcomeModel();
				
				var caseModel = await _caseModelService.GetCaseByUrn(CaseUrn);
				Division = caseModel.Division;

				LoadPageComponents(DecisionOutcome);

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

				DecisionOutcome.Status = (DecisionOutcomeStatus)DecisionOutcomeStatus.SelectedId;
				DecisionOutcome.DecisionMadeDate = DecisionMadeDate.Date;
				DecisionOutcome.DecisionEffectiveFromDate = DecisionEffectiveFromDate.Date;
				DecisionOutcome.Authorizer = DecisionOutcomeAuthorizer.SelectedId.HasValue
					? (DecisionOutcomeAuthorizer)DecisionOutcomeAuthorizer.SelectedId : null;

				if (OutcomeId.HasValue)
				{
					var updateDecisionOutcomeRequest = DecisionMapping.ToUpdateDecisionOutcome(DecisionOutcome);
					await _decisionService.PutDecisionOutcome(CaseUrn, DecisionId, updateDecisionOutcomeRequest);

					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}

				var request = DecisionMapping.ToCreateDecisionOutcomeRequest(DecisionOutcome);

				await _decisionService.PostDecisionOutcome(CaseUrn, DecisionId, request);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(EditDecisionOutcomeModel model)
		{
			LoadPageComponents();

			if (model == null) return;

			DecisionOutcomeStatus.SelectedId = (int)model.Status;
			DecisionMadeDate.Date = model.DecisionMadeDate;
			DecisionEffectiveFromDate.Date = model.DecisionEffectiveFromDate;
			DecisionOutcomeAuthorizer.SelectedId = model.Authorizer.HasValue ? (int)model.Authorizer : null;
		}

		private void LoadPageComponents()
		{
			SetupPage();

			DecisionOutcomeStatus = BuildDecisionOutcomeStatusComponent(DecisionOutcomeStatus?.SelectedId);
			DecisionMadeDate = BuildDecisionMadeDateComponent(DecisionMadeDate?.Date);
			DecisionEffectiveFromDate = BuildDecisionEffectiveFromDateComponent(DecisionEffectiveFromDate?.Date);
			DecisionOutcomeAuthorizer = BuildDecisionAuthorizerComponent(DecisionOutcomeAuthorizer?.SelectedId);
			BusinessAreaCheckBoxes = Division == API.Contracts.Case.Division.RegionsGroup ? BuildRegionsGroupBusinessAreaCheckboxes() : BuildBusinessAreaCheckBoxes();
		}

		private List<DecisionOutcomeBusinessArea> BuildRegionsGroupBusinessAreaCheckboxes()
		{
			return new List<DecisionOutcomeBusinessArea>
			{
				DecisionOutcomeBusinessArea.SchoolsFinancialSupportAndOversight,
				DecisionOutcomeBusinessArea.RegionsGroup
			};
		}

		private List<DecisionOutcomeBusinessArea> BuildBusinessAreaCheckBoxes()
		{
			return Enum.GetValues<DecisionOutcomeBusinessArea>().ToList();
		}

		private void SetupPage()
		{
			ViewData[ViewDataConstants.Title] = OutcomeId.HasValue ? "Edit outcome" : "Add outcome";
			SaveAndContinueButtonText = OutcomeId.HasValue ? "Save and return to decision" : "Save and return to case overview";
		}

		private async Task<EditDecisionOutcomeModel> CreateDecisionOutcomeModel()
		{
			var apiDecision = await _decisionService.GetDecision(CaseUrn, (int)DecisionId);
			var result = DecisionMapping.ToEditDecisionOutcomeModel(apiDecision);

			return result;
		}

		private static RadioButtonsUiComponent BuildDecisionOutcomeStatusComponent(int? selectedId = null)
		{
			var radioItems = Enum.GetValues(typeof(DecisionOutcomeStatus))
				.Cast<DecisionOutcomeStatus>()
				.Select(v =>
				{
					return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
				}).ToArray();

			return new(ElementRootId: "decision-outcome-status", Name: nameof(DecisionOutcomeStatus), "What was the decision outcome?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "decision outcome",
				ErrorTextForRequiredField = "Select a decision outcome",
				SortOrder = 1
			};
		}

		private static RadioButtonsUiComponent BuildDecisionAuthorizerComponent(int? selectedId = null)
		{
			var radioItems = Enum.GetValues(typeof(DecisionOutcomeAuthorizer))
				.Cast<DecisionOutcomeAuthorizer>()
				.Select(v =>
				{
					return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
				}).ToArray();

			return new(ElementRootId: "decision-outcome-authorizer", Name: nameof(DecisionOutcomeAuthorizer), "Who authorised this decision?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "decision outcome authorizer"
			};
		}

		private static OptionalDateTimeUiComponent BuildDecisionMadeDateComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("decision-made", nameof(DecisionMadeDate), "When was the decision made?")
			{
				Date = date,
				DisplayName = "Date decision was made",
				SortOrder = 2
			};
		}

		private static OptionalDateTimeUiComponent BuildDecisionEffectiveFromDateComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("take-effect", nameof(DecisionEffectiveFromDate), "When did or does the decision take effect?")
			{
				Date = date,
				DisplayName = "Date decision takes effect",
				SortOrder = 3
			};
		}
	}
}