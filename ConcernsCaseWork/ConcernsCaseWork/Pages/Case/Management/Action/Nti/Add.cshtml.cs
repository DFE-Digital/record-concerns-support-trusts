using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
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
		public RadioButtonsUiComponent NtiStatus { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DateIssued { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		public NtiModel Nti { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "NtiId")]
		public long? NtiId { get; set; }

		public string CancelLinkUrl { get; set; }

		public AddPageModel(
			INtiModelService ntiWarningLetterModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiModelService = ntiWarningLetterModelService;
			_logger = logger;
		}


		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!IsReturningFromConditions) // this is a fresh request, not a return from the conditions page.
				{
					ContinuationId = string.Empty;
				}

				Nti = await LoadNti();

				if (Nti is { IsClosed: true })
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}");
				}

				LoadPageComponents(Nti);

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
			try
			{
				if (!ModelState.IsValid)
				{
					LoadPageComponents();
					return Page();
				}

				if (action.Equals(ActionForAddConditionsButton, StringComparison.OrdinalIgnoreCase))
				{
					return await HandOverToConditions();
				}

				return await HandleContinue();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private async Task<NtiModel> LoadNti()
		{
			if (HasCachedNti(CaseUrn, ContinuationId))
			{
				return await LoadWarningLetterFromCache();
			}
			else if (NtiId != null)
			{
				return await LoadWarningLetterFromDB();
			}

			return new NtiModel();
		}

		private static bool HasCachedNti(int caseUrn, string continuationId)
		{
			return !string.IsNullOrWhiteSpace(continuationId) && continuationId.StartsWith(caseUrn.ToString());
		}

		private async Task<NtiModel> LoadWarningLetterFromCache()
		{
			return await _ntiModelService.GetNtiFromCacheAsync(ContinuationId);
		}

		private async Task<NtiModel> LoadWarningLetterFromDB()
		{
			return await _ntiModelService.GetNtiByIdAsync(NtiId.Value);
		}

		private void LoadPageComponents(NtiModel model)
		{
			LoadPageComponents();

			Notes.Text.StringContents = model.Notes;
			NtiStatus.SelectedId = (int?)model.Status;
			Reasons = BuildReasonsComponent(model.Reasons);

			if (model.DateStarted.HasValue)
			{
				DateIssued.Date = new OptionalDateModel(model.DateStarted.Value);
			}
		}

		private void LoadPageComponents()
		{
			CancelLinkUrl = NtiId.HasValue ? @$"/case/{CaseUrn}/management/action/nti/{NtiId.Value}"
							 : @$"/case/{CaseUrn}/management/action";

			Notes = BuildNotesComponent(Notes?.Text.StringContents);
			NtiStatus = BuildStatusComponent(NtiStatus?.SelectedId);
			Reasons = BuildReasonsComponent(GetSelectedReasons().ToList());
			DateIssued = BuildDateIssuedComponent(DateIssued?.Date);
		}

		private static OptionalDateTimeUiComponent BuildDateIssuedComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("date-issued", nameof(DateIssued), "Date NTI issued")
			{
				Date = date,
				DisplayName = "Date NTI issued"
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

		private async Task<RedirectResult> HandleContinue()
		{
			var ntiModel = await GetUpToDateModel();

			if (NtiId == null)
			{
				await _ntiModelService.CreateNtiAsync(ntiModel);
			}
			else
			{
				await _ntiModelService.PatchNtiAsync(ntiModel);
			}

			TempData.Remove(nameof(ContinuationId));

			return Redirect($"/case/{CaseUrn}/management");
		}

		private async Task<RedirectResult> HandOverToConditions()
		{
			var ntiModel = await GetUpToDateModel();

			if (!HasCachedNti(CaseUrn, ContinuationId))
			{
				ContinuationId = $"{CaseUrn}_{Guid.NewGuid()}";
			}

			await _ntiModelService.StoreNtiAsync(ntiModel, ContinuationId);

			TempData.Keep(nameof(ContinuationId));

			if (NtiId == null)
			{
				return Redirect($"/case/{CaseUrn}/management/action/nti/conditions");
			}

			return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}/edit/conditions");
		}

		private async Task<NtiModel> GetUpToDateModel()
		{
			NtiModel nti;

			if (HasCachedNti(CaseUrn, ContinuationId)) // conditions have been recorded
			{
				nti = await _ntiModelService.GetNtiFromCacheAsync(ContinuationId);
				nti = PopulateNtiFromRequest(nti);
			}
			else if (NtiId.HasValue)
			{
				nti = await _ntiModelService.GetNtiByIdAsync(NtiId.Value);
				nti = PopulateNtiFromRequest(nti);
			}
			else
			{
				nti = PopulateNtiFromRequest();
			}

			if (NtiId != null)
			{
				nti.Id = NtiId.Value;
			}

			return nti;
		}

		private static RadioButtonsUiComponent BuildStatusComponent(int? selectedId = null)
		{
			var enumValues = new List<NtiStatus>()
			{
				API.Contracts.NoticeToImprove.NtiStatus.PreparingNTI,
				API.Contracts.NoticeToImprove.NtiStatus.IssuedNTI,
				API.Contracts.NoticeToImprove.NtiStatus.ProgressOnTrack,
				API.Contracts.NoticeToImprove.NtiStatus.EvidenceOfNTINonCompliance,
				API.Contracts.NoticeToImprove.NtiStatus.SeriousNTIBreaches,
				API.Contracts.NoticeToImprove.NtiStatus.SubmissionToLiftNTIInProgress,
				API.Contracts.NoticeToImprove.NtiStatus.SubmissionToCloseNTIInProgress
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = $"status-{v.Description()}" };
			}).ToArray();

			return new(ElementRootId: "status", Name: nameof(NtiStatus), "Current status")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "Status",
			};
		}

		private IEnumerable<RadioItem> BuildReasonsComponent(ICollection<NtiReason> selectedReasons)
		{
			var reasons = Enum.GetValues(typeof(NtiReason)).Cast<NtiReason>();
			return reasons.Select(r => new RadioItem
			{
				Id = ((int)r).ToString(),
				Text = r.Description(),
				IsChecked = selectedReasons.Any(wl_r => wl_r == r)
			});
		}

		private NtiModel PopulateNtiFromRequest(NtiModel ntiModel)
		{
			var newValues = PopulateNtiFromRequest();

			ntiModel.CaseUrn = newValues.CaseUrn;
			ntiModel.Reasons = newValues.Reasons;
			ntiModel.Status = newValues.Status;
			ntiModel.Notes = newValues.Notes;
			ntiModel.DateStarted = newValues.DateStarted;
			ntiModel.CreatedAt = newValues.CreatedAt;
			ntiModel.UpdatedAt = newValues.UpdatedAt;

			return ntiModel;
		}

		private NtiModel PopulateNtiFromRequest()
		{
			var nti = new NtiModel()
			{
				CaseUrn = CaseUrn,
				Reasons = GetSelectedReasons().ToArray(),
				Status = (NtiStatus?)NtiStatus.SelectedId,
				Conditions = Array.Empty<NtiConditionModel>(),
				Notes = Notes.Text.StringContents,
				DateStarted = DateIssued.Date.ToDateTime(),
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};

			return nti;
		}

		private IEnumerable<NtiReason> GetSelectedReasons()
		{
			if (!Request.HasFormContentType)
			{
				return new List<NtiReason>();
			}

			var reasons = Request.Form["reason"];

			return reasons.Select(r => (NtiReason)int.Parse(r));
		}
	}
}