using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly ITargetedTrustEngagementService _targetedTrustEngagementService;
		private readonly ILogger<ClosePageModel> _logger;

		[BindProperty]
		public OptionalDateTimeUiComponent EngagementEndDate { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent EngagementOutcomeComponent { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		[BindProperty(SupportsGet = true)]
		public int TargetedtrustengagementId { get; set; }

		[BindProperty(Name="Urn", SupportsGet = true)]
		public int CaseUrn { get; set; }

		public ClosePageModel(ITargetedTrustEngagementService targetedTrustEngagementService, ILogger<ClosePageModel> logger)
		{
			_targetedTrustEngagementService = targetedTrustEngagementService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var tte = await _targetedTrustEngagementService.GetTargetedTrustEngagement(CaseUrn, TargetedtrustengagementId);

				if (!IsTargetedTrustEngagementEditable(tte))
				{
					return Redirect($"/case/{CaseUrn}/management/action/targetedtrustengagement/{TargetedtrustengagementId}");
				}

				LoadPageComponents(tte);
								
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

				var closeTargetedTrustEngagementRequest = new CloseTargetedTrustEngagementRequest 
				{ 
					EngagementEndDate = EngagementEndDate.Date?.ToDateTime() ?? null,
					OutcomeId = (TargetedTrustEngagementOutcome)EngagementOutcomeComponent.SelectedId,
					Notes = Notes.Text.StringContents
				};

				await _targetedTrustEngagementService.CloseTargetedTrustEngagement(CaseUrn, TargetedtrustengagementId, closeTargetedTrustEngagementRequest);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(GetTargetedTrustEngagementResponse model)
		{
			LoadPageComponents();

			Notes.Text.StringContents = model.Notes;
		}

		private void LoadPageComponents()
		{
			EngagementEndDate = BuildEngagementEndDateComponent();
			EngagementOutcomeComponent = BuildEngagementOutcomeComponent();
			Notes = BuildNotesComponent(Notes?.Text.StringContents);
		}

		private static OptionalDateTimeUiComponent BuildEngagementEndDateComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("engagement-end", nameof(EngagementEndDate), "What date did the engagement end?")
			{
				Date = date,
				DisplayName = "Date engagement end",
				Required = false
			};
		}


		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("SupportingNotes", nameof(Notes), "Notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action.",
			Text = new ValidateableString()
			{
				MaxLength = TargetedTrustEngagementConstants.MaxSupportingNotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};

		private static List<EngagementOutcomeQuestionModel> BuildEngagementOutcomesList()
		{
			var outcomes = new List<EngagementOutcomeQuestionModel>()
			{
				new EngagementOutcomeQuestionModel()
				{
					Id = TargetedTrustEngagementOutcome.AdequateResponseReceived
				},
				new EngagementOutcomeQuestionModel()
				{
					Id = TargetedTrustEngagementOutcome.InadequateResponseReceived,
					Hint = "Concern will be raised or updated."
				},
				new EngagementOutcomeQuestionModel()
				{
					Id = TargetedTrustEngagementOutcome.NoEngagementTookPlace,
					Hint = "Activities ruled out at triage stage."
				},
				new EngagementOutcomeQuestionModel()
				{
					Id = TargetedTrustEngagementOutcome.NoResponseRequired
				}
			};

			return outcomes;
		}

		private static RadioButtonsUiComponent BuildEngagementOutcomeComponent()
		{
			var result = new RadioButtonsUiComponent("case-tte-outcome", nameof(EngagementOutcomeComponent), "");

			result.ErrorTextForRequiredField = "Select an outcome for the engagement";
			result.Required = true;

			var outcomes = BuildEngagementOutcomesList();

			result.RadioItems = outcomes.Select(value =>
			{
				var radioItem = new SimpleRadioItem(value.Id.Description(), (int)value.Id)
				{
					TestId = value.Id.Description(),
					HintText = value.Hint,
				};

				return radioItem;
			});

			return result;
		}

		private static bool IsTargetedTrustEngagementEditable(GetTargetedTrustEngagementResponse tte) => !IsTargetedTrustEngagementClosed(tte);
		private static bool IsTargetedTrustEngagementClosed(GetTargetedTrustEngagementResponse tte) => tte.ClosedAt.HasValue;
	}

	public class EngagementOutcomeQuestionModel
	{
		public string Name { get; set; }
		public TargetedTrustEngagementOutcome Id { get; set; }
		public string Hint { get; set; }
	}
}