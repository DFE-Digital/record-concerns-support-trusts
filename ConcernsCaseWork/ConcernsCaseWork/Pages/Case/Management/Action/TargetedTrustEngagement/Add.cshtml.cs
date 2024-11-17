using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Utils.Extensions;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Mappers;

#nullable disable

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ITargetedTrustEngagementService _targetedTrustEngagementService;
		private readonly ILogger<AddPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "targetedtrustengagementId")]
		public int? TargetedTrustEngagementId { get; set; }

		[BindProperty]
		public CreateTargetedTrustEngagementRequest TargetedTrustEngagement { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent EngagementStartDate { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent EngagementActivitiesComponent { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }


		public string SaveAndContinueButtonText { get; set; }

		public AddPageModel(
			ITargetedTrustEngagementService targetedTrustEngagementService,
			ILogger<AddPageModel> logger)
		{
			_targetedTrustEngagementService = targetedTrustEngagementService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				TargetedTrustEngagement = await CreateTargetedTrustEngagementModel();

				LoadPageComponents(TargetedTrustEngagement);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
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

				TargetedTrustEngagement.EngagementStartDate = EngagementStartDate.Date?.ToDateTime() ?? null;
				TargetedTrustEngagement.Notes = Notes.Text.StringContents;
				TargetedTrustEngagement.ActivityId = (TargetedTrustEngagementActivity?)EngagementActivitiesComponent.SelectedId;
				TargetedTrustEngagement.ActivityTypes = MapToEngagementActivityTypes(EngagementActivitiesComponent);
				TargetedTrustEngagement.CreatedBy = User.Identity.Name;

				if (TargetedTrustEngagementId.HasValue)
				{
					var updateTargetedTrustEngagementRequest = TargetedTrustEngagementMapping.ToTargetedTrustEngagementRequest(TargetedTrustEngagement);
					await _targetedTrustEngagementService.PutTargetedTrustEngagement(CaseUrn, (int)TargetedTrustEngagementId, updateTargetedTrustEngagementRequest);

					return Redirect($"/case/{CaseUrn}/management/action/targetedtrustengagement/{TargetedTrustEngagementId}");
				}

				await _targetedTrustEngagementService.PostTargetedTrustEngagement(TargetedTrustEngagement);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(CreateTargetedTrustEngagementRequest model)
		{
			LoadPageComponents();

			EngagementStartDate.Date = new OptionalDateModel()
			{
				Day = model.EngagementStartDate?.Day.ToString("00"),
				Month = model.EngagementStartDate?.Month.ToString("00"),
				Year = model.EngagementStartDate?.Year.ToString()
			};

			Notes.Text.StringContents = model.Notes;

			SetEngagementActivityAnswers(model);

		}

		private void SetEngagementActivityAnswers(CreateTargetedTrustEngagementRequest model)
		{
			EngagementActivitiesComponent.SelectedId = (int?)model.ActivityId;

			if (model.ActivityId == TargetedTrustEngagementActivity.NoEngagementActivitiesWereTakenForward)
			{
				EngagementActivitiesComponent.SelectedSubIds = model.ActivityTypes.Select(a => (int)a).ToList();
			}
			else
			{
				EngagementActivitiesComponent.SelectedSubId = (int?)model.ActivityTypes?.FirstOrDefault();
			}
		}

		private void LoadPageComponents()
		{
			SetupPage();

			EngagementStartDate = BuildEngagementStartDateComponent(EngagementStartDate?.Date);
			EngagementActivitiesComponent = BuildEngagementActivitiesComponent();
			Notes = BuildNotesComponent(Notes?.Text.StringContents);
		}

		private void SetupPage()
		{
			ViewData[ViewDataConstants.Title] = TargetedTrustEngagementId.HasValue ? "Edit TTE" : "Add TTE";
			SaveAndContinueButtonText = TargetedTrustEngagementId.HasValue ? "Save and return to engagement" : "Save and return to case overview";
		}

		private async Task<CreateTargetedTrustEngagementRequest> CreateTargetedTrustEngagementModel()
		{
			var result = new CreateTargetedTrustEngagementRequest();

			result.CaseUrn = (int)CaseUrn;

			if (TargetedTrustEngagementId.HasValue)
			{
				var apiEngagement = await _targetedTrustEngagementService.GetTargetedTrustEngagement(CaseUrn, (int)TargetedTrustEngagementId);

				result = TargetedTrustEngagementMapping.ToEditTTEModel(apiEngagement);
			}

			return result;
		}

		private TargetedTrustEngagementActivityType[] MapToEngagementActivityTypes(RadioButtonsUiComponent radioComponent)
		{
			var subIDs = new List<TargetedTrustEngagementActivityType>();

			if (radioComponent.SelectedSubId.HasValue)
			{
				subIDs.Add((TargetedTrustEngagementActivityType)radioComponent.SelectedSubId);
			}

			if (radioComponent.SelectedSubIds != null)
			{
				var types = radioComponent.SelectedSubIds.Select(r =>
				{
					return (TargetedTrustEngagementActivityType)r;
				});

				subIDs.AddRange(types);
			}

			return subIDs.ToArray();
		}

		private static OptionalDateTimeUiComponent BuildEngagementStartDateComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("engagement-start", nameof(EngagementStartDate), "What date did the engagement start?")
			{
				Date = date,
				DisplayName = "Date engagement start"
			};
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("case-tte-notes", nameof(Notes), "Notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action.",
			Text = new ValidateableString()
			{
				MaxLength = TargetedTrustEngagementConstants.MaxSupportingNotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};

		private static List<EngagementActivityQuestionModel> BuildEngagementActivitiesList()
		{
			var activities = new List<EngagementActivityQuestionModel>()
			{
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven,
					Hint = "Engagement with any trusts identified at potential risk from the latest financial returns.",
					SubOptions = new List<SubEngagementActivityModel>()
					{
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Category1),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Category2),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Category3),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Category4)
					}
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.ExecutivePayEngagement,
					Hint = "Contacting trusts that have flagged for high pay when compared with trusts of similar size and type.",
					SubOptions = new List<SubEngagementActivityModel>()
					{
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.CEOs),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Leadership),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.CEOsAndLeadership)
					}
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.FinancialReturnsAssurance,
					Hint = "Following up with trusts on issues flagged in their latest financial statements.",
					SubOptions = new List<SubEngagementActivityModel>()
					{
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.AnnualSummaryInternalScrutinyReports),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.AuditIssues),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.ManagementLetterIssues),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.RegularityIssues)
					}
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.ReservesOversightAndAssuranceProject,
					Hint = "Engaging trusts holding substantial levels of unallocated reserves and clarifying the plans they have for those funds.",
					HintLinkTitle = "SFSO Knowledge: ROAP",
					HintLink = "https://educationgovuk.sharepoint.com/sites/lveefa00003/SitePages/Reserves--Oversight-%26-Assurance-Project-(ROAP).aspx?csf=1&web=1&e=yomR4Z&cid=59072d93-d6cb-4cf8-bd89-c81e2e3891e5#process",
					SubOptions = new List<SubEngagementActivityModel>()
					{
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Priority1),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Priority2),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Priority3),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.Priority4)
					}
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.LocalProactiveEngagement,
					Hint = "Division-led work to target trusts with potential financial concerns."
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.OtherNationalProcesses,
					Hint = "Other activities to target trusts with potential financial concerns (e.g. declining pupil numbers)."
				},
				new EngagementActivityQuestionModel()
				{
					Id = TargetedTrustEngagementActivity.NoEngagementActivitiesWereTakenForward,
					Hint = "If engagement was considered but not taken forward, select which of the following activities were \r\ntriaged. Pick all that apply.\r\n",
					SubOptions = new List<SubEngagementActivityModel>()
					{
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.BudgetForecastReturnAccountsReturnDriven, "Engagement with any trusts identified at potential risk from the latest financial returns."),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.ExecutivePayEngagement, "Contacting trusts that have flagged for high pay when compared with trusts of similar size and type."),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.FinancialReturnsAssurance, "Following up with trusts on issues flagged in their latest financial statements."),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.ReservesOversightAssuranceProject,  "Engaging trusts holding substantial levels of unallocated reserves and clarifying the plans they have for those funds."),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.LocalProactiveEngagement,  "Division-led work to target trusts with potential financial concerns."),
						new SubEngagementActivityModel(TargetedTrustEngagementActivityType.OtherNationalProcesses,  "Other national processes to target trusts with potential financial concerns (e.g. declining pupil \r\nnumbers)."),
					}
				}
			};

			return activities;
		}

		private static RadioButtonsUiComponent BuildEngagementActivitiesComponent()
		{
			var result = new RadioButtonsUiComponent("case-tte-activities", nameof(EngagementActivitiesComponent), "");

			result.SubItemDisplayName = "Engagement Activity Type";
			result.SubOptionsAlwaysShown = true;
			result.Required = true;
			result.ErrorTextForRequiredField = "Select Engagement Activity";

			var activities = BuildEngagementActivitiesList();

			result.RadioItems = activities.Select(value =>
			{
				var radioItem = new SimpleRadioItem(value.Id.Description(), (int)value.Id)
				{
					TestId = value.Id.Description(),
					HintText = value.Hint,
					HintLink = value.HintLink,
					HintLinkTitle = value.HintLinkTitle,
				};

				if (value.Id.Equals(TargetedTrustEngagementActivity.NoEngagementActivitiesWereTakenForward))
				{
					radioItem.SubCheckboxItems = value.SubOptions.Select(s =>
					{
						var subRadioItem = new SubCheckboxItem((int)s.Id, s.Id.Description(), s.Id.Description());
						subRadioItem.Text = s.Id.Description();
						subRadioItem.HintText = s.Hint;

						return subRadioItem;
					}).ToList();
				}
				else
				{
					radioItem.SubRadioItems = value.SubOptions.Select(s =>
					{
						var subRadioItem = new SubRadioItem(s.Id.Description(), (int)s.Id);

						subRadioItem.TestId = s.Id.Description();

						return subRadioItem;
					}).ToList();
				}

				return radioItem;
			});

			return result;
		}
	
	}

	public class EngagementActivityQuestionModel
	{
		public string Name { get; set; }
		public TargetedTrustEngagementActivity? Id { get; set; }
		public string Hint { get; set; }
		public string HintLink { get; set; }
		public string HintLinkTitle { get; set; }
		public bool IsChecked { get; set; }

		public List<SubEngagementActivityModel> SubOptions { get; set; } = new List<SubEngagementActivityModel>();
	}

	public class SubEngagementActivityModel
	{
		public SubEngagementActivityModel(TargetedTrustEngagementActivityType? id)
		{
			Id = id;
		}

		public SubEngagementActivityModel(TargetedTrustEngagementActivityType? id, string hint)
		{
			Id = id;
			Hint = hint;
		}
		
		public TargetedTrustEngagementActivityType? Id { get; set; }
		public string Hint { get; set; }
		public bool IsChecked { get; set; }
	}

}