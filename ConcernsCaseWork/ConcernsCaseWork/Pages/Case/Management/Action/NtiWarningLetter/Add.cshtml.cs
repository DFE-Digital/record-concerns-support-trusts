
using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiWarningLetterConditionsCachedService _ntiWarningLetterConditionsCachedService;
		private readonly ILogger<AddPageModel> _logger;

		public string ActionForAddConditionsButton = "add-conditions";
		public string ActionForContinueButton = "continue";

		[TempData]
		public string ContinuationId { get; set; }

		[TempData]
		public bool IsReturningFromConditions { get; set; }

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> Reasons { get; private set; }

		[BindProperty]
		public RadioButtonsUiComponent NtiWarningLetterStatus { get; set; } = BuildStatusComponent();

		[BindProperty]
		public OptionalDateTimeUiComponent SentDate { get; set; } = BuildDateSentComponent();

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

		public NtiWarningLetterModel? WarningLetter { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "warningLetterId")]
		public int? WarningLetterId { get; set; }

		public string CancelLinkUrl { get; set; }

		public AddPageModel(
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterConditionsCachedService ntiWarningLetterConditionsCachedService,
			ILogger<AddPageModel> logger)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterConditionsCachedService = ntiWarningLetterConditionsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				WarningLetter = new NtiWarningLetterModel();

				if (!IsReturningFromConditions) // this is a fresh request, not a return from the conditions page.
				{
					ContinuationId = string.Empty;
				}

				if (!string.IsNullOrWhiteSpace(ContinuationId) && ContinuationId.StartsWith(CaseUrn.ToString()))
				{
					await LoadWarningLetterFromCache();
				}
				else
				{
					if (WarningLetterId != null)
					{
						await LoadWarningLetterFromDB();
					}
				}
				
				if (WarningLetter is { IsClosed: true })
				{
					return Redirect($"/case/{CaseUrn}/management/action/ntiwarningletter/{WarningLetterId}");
				}

				LoadComponents(WarningLetter);

				TempData.Keep(nameof(ContinuationId));
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					return Page();
				}

				if (action.Equals(ActionForAddConditionsButton, StringComparison.OrdinalIgnoreCase))
				{
					return await HandOverToConditions();
				}
				else if (action.Equals(ActionForContinueButton, StringComparison.OrdinalIgnoreCase))
				{
					return await HandleContinue();
				}
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void SetupPage()
		{
			CancelLinkUrl = WarningLetterId.HasValue ? @$"/case/{CaseUrn}/management/action/ntiwarningletter/{WarningLetterId.Value}"
							 : @$"/case/{CaseUrn}/management/action";
		}

		private void LoadComponents(NtiWarningLetterModel warningLetterModel)
		{
			SetupPage();

			NtiWarningLetterStatus.SelectedId = (int?)warningLetterModel.Status;
			Notes.Text.StringContents = warningLetterModel.Notes;

			if (warningLetterModel.SentDate.HasValue)
			{
				SentDate.Date = new OptionalDateModel(warningLetterModel.SentDate.Value);
			}

			Reasons = GetReasons();
		}

		private void ResetOnValidationError()
		{
			SetupPage();

			NtiWarningLetterStatus = BuildStatusComponent(NtiWarningLetterStatus.SelectedId);
			SentDate = BuildDateSentComponent(SentDate.Date);
			Notes = BuildNotesComponent(Notes.Text.StringContents);
			Reasons = GetReasons();
		}

		private async Task<RedirectResult> HandleContinue()
		{
			var ntiModel = await GetUpToDateModel();

			if (WarningLetterId == null)
			{
				await _ntiWarningLetterModelService.CreateNtiWarningLetter(ntiModel);
			}
			else
			{
				await _ntiWarningLetterModelService.PatchNtiWarningLetter(ntiModel);
			}

			TempData.Remove(nameof(ContinuationId));
			return Redirect($"/case/{CaseUrn}/management");
		}

		private async Task<RedirectResult> HandOverToConditions()
		{
			var ntiModel = await GetUpToDateModel();
			if (string.IsNullOrWhiteSpace(ContinuationId) || !ContinuationId.StartsWith(CaseUrn.ToString()))
			{
				ContinuationId = $"{CaseUrn}_{Guid.NewGuid()}";
			}

			await _ntiWarningLetterModelService.StoreWarningLetter(ntiModel, ContinuationId);

			TempData.Keep(nameof(ContinuationId));
			if (WarningLetterId == null)
			{
				return Redirect($"/case/{CaseUrn}/management/action/NtiWarningLetter/conditions");
			}
			else
			{
				return Redirect($"/case/{CaseUrn}/management/action/NtiWarningLetter/{WarningLetterId}/edit/conditions");
			}
		}

		private async Task<NtiWarningLetterModel> GetUpToDateModel()
		{
			NtiWarningLetterModel nti = null;

			if (!string.IsNullOrWhiteSpace(ContinuationId)) // conditions have been recorded
			{
				if (ContinuationId.StartsWith(CaseUrn.ToString()))
				{
					nti = await _ntiWarningLetterModelService.GetWarningLetter(ContinuationId);
					nti = PopulateNtiFromRequest(nti); // populate current form values on top of values recorded in conditions form
				}
			}
			else if (WarningLetterId.HasValue)
			{
				nti = await _ntiWarningLetterModelService.GetNtiWarningLetterId(WarningLetterId.Value);
				nti = PopulateNtiFromRequest(nti);
			}
			else
			{
				// creating a new nti
				nti = PopulateNtiFromRequest();
				await SetDefaults(nti);
			}

			if (WarningLetterId != null)
			{
				nti.Id = WarningLetterId.Value;
			}

			return nti;
		}

		private async Task<NtiWarningLetterModel> SetDefaults(NtiWarningLetterModel ntiWarningLetterModel)
		{
			var conditions = await _ntiWarningLetterConditionsCachedService.GetAllConditionsAsync();
			var financialReturnsCondition = NtiWarningLetterMappers.ToServiceModel(conditions.Single(c => c.Id == (int)NtiWarningLetterCondition.FinancialReturns));
			ntiWarningLetterModel.Conditions.Add(financialReturnsCondition);

			return ntiWarningLetterModel;
		}

		private static RadioButtonsUiComponent BuildStatusComponent(int? selectedId = null)
		{
			var enumValues = new List<NtiWarningLetterStatus>()
			{
				API.Contracts.NtiWarningLetter.NtiWarningLetterStatus.PreparingWarningLetter,
				API.Contracts.NtiWarningLetter.NtiWarningLetterStatus.SentToTrust
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "status", Name: nameof(NtiWarningLetterStatus), "Current status")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "Status",
			};
		}

		private static OptionalDateTimeUiComponent BuildDateSentComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("date-sent", nameof(SentDate), "Date warning letter sent")
			{
				Date = date,
				DisplayName = "Date warning letter sent"
			};
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("nti-notes", nameof(Notes), "Notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action.",
			Text = new ValidateableString()
			{
				MaxLength = 2000,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};

		private IEnumerable<RadioItem> GetReasons()
		{
			var reasons = Enum.GetValues(typeof(NtiWarningLetterReason)).Cast<NtiWarningLetterReason>();
			return reasons.Select(r => new RadioItem
			{
				Id = ((int)r).ToString(),
				Text = r.Description(),
				IsChecked = WarningLetter?.Reasons?.Any(wl_r => wl_r == r) ?? false
			});
		}

		private NtiWarningLetterModel PopulateNtiFromRequest(NtiWarningLetterModel ntiWarningLetterModel)
		{
			if (ntiWarningLetterModel == null)
			{
				throw new ArgumentException(nameof(ntiWarningLetterModel));
			}

			var newValues = PopulateNtiFromRequest();

			ntiWarningLetterModel.CaseUrn = newValues.CaseUrn;
			ntiWarningLetterModel.Reasons = newValues.Reasons;
			ntiWarningLetterModel.Status = newValues.Status;
			ntiWarningLetterModel.Notes = newValues.Notes;
			ntiWarningLetterModel.SentDate = newValues.SentDate;
			ntiWarningLetterModel.CreatedAt = newValues.CreatedAt;
			ntiWarningLetterModel.UpdatedAt = newValues.UpdatedAt;

			return ntiWarningLetterModel;
		}

		private NtiWarningLetterModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];

			var nti = new NtiWarningLetterModel()
			{
				CaseUrn = CaseUrn,
				Reasons = reasons.Select(r => (NtiWarningLetterReason)int.Parse(r)).ToArray(),
				Status = (NtiWarningLetterStatus?)NtiWarningLetterStatus.SelectedId,
				Conditions = new List<NtiWarningLetterConditionModel>(),
				Notes = Notes.Text.StringContents,
				SentDate = SentDate.Date.ToDateTime(),
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};

			return nti;
		}

		private async Task LoadWarningLetterFromCache()
		{
			WarningLetter = await _ntiWarningLetterModelService.GetWarningLetter(ContinuationId);
		}

		private async Task LoadWarningLetterFromDB()
		{
			WarningLetter = await _ntiWarningLetterModelService.GetNtiWarningLetterId(WarningLetterId.Value);
		}
	}
}