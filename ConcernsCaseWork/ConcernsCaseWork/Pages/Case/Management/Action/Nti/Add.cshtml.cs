using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Services.Nti;
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
		private readonly INtiReasonsCachedService _ntiReasonsCachedService;
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<AddPageModel> _logger;

		public string ActionForAddConditionsButton = "add-conditions";
		public string ActionForContinueButton = "continue";

		[TempData]
		public string ContinuationId { get; set; }

		[TempData]
		public bool IsReturningFromConditions { get; set; }

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> Statuses { get; private set; }
		public IEnumerable<RadioItem> Reasons { get; private set; }

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
			INtiReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiModelService ntiWarningLetterModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiReasonsCachedService = ntiWarningLetterReasonsCachedService;
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

				if (!string.IsNullOrWhiteSpace(ContinuationId) && ContinuationId.StartsWith(CaseUrn.ToString()))
				{
					await LoadWarningLetterFromCache();
				}
				else
				{
					if (NtiId != null)
					{
						await LoadWarningLetterFromDB();
					}
				}

				if (Nti is {IsClosed : true})
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}");
				}

				await LoadPageComponents(Nti);

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
					await LoadPageComponents();
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

		private async Task SetupPage()
		{
			Statuses = GetStatuses();
			Reasons = await GetReasons();

			CancelLinkUrl = NtiId.HasValue ? @$"/case/{CaseUrn}/management/action/nti/{NtiId.Value}"
										 : @$"/case/{CaseUrn}/management/action";
		}

		private async Task LoadPageComponents(NtiModel model)
		{
			await LoadPageComponents();

			if (model == null)
			{
				return;
			}

			Notes.Text.StringContents = model.Notes;

			if (model.DateStarted.HasValue)
			{
				DateIssued.Date = new OptionalDateModel(model.DateStarted.Value);
			}
		}

		private async Task LoadPageComponents()
		{
			await SetupPage();

			DateIssued = BuildDateIssuedComponent(DateIssued?.Date);
			Notes = BuildNotesComponent(Notes?.Text.StringContents);
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
			if (string.IsNullOrWhiteSpace(ContinuationId) || !ContinuationId.StartsWith(CaseUrn.ToString()))
			{
				ContinuationId = $"{CaseUrn}_{Guid.NewGuid()}";
			}

			await _ntiModelService.StoreNtiAsync(ntiModel, ContinuationId);

			TempData.Keep(nameof(ContinuationId));
			if (NtiId == null)
			{
				return Redirect($"/case/{CaseUrn}/management/action/nti/conditions");
			}
			else
			{
				return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}/edit/conditions");
			}
		}

		private async Task<NtiModel> GetUpToDateModel()
		{
			NtiModel nti = null;

			if (!string.IsNullOrWhiteSpace(ContinuationId)) // conditions have been recorded
			{
				if (ContinuationId.StartsWith(CaseUrn.ToString()))
				{
					nti = await _ntiModelService.GetNtiAsync(ContinuationId);
					nti = PopulateNtiFromRequest(nti); // populate current form values on top of values recorded in conditions form
				}
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

		private IEnumerable<RadioItem> GetStatuses()
		{
			var statuses = new List<NtiStatus>
			{
				NtiStatus.PreparingNTI,
				NtiStatus.IssuedNTI,
				NtiStatus.ProgressOnTrack,
				NtiStatus.EvidenceOfNTINonCompliance,
				NtiStatus.SeriousNTIBreaches,
				NtiStatus.SubmissionToLiftNTIInProgress,
				NtiStatus.SubmissionToCloseNTIInProgress
			};

			return statuses.Select(s => new RadioItem
			{
				Id = ((int)s).ToString(),
				Text = s.Description(),
				IsChecked = Nti?.Status == s
			});
		}

		private async Task<IEnumerable<RadioItem>> GetReasons()
		{
			var reasons = await _ntiReasonsCachedService.GetAllReasonsAsync();
			return reasons.Select(r => new RadioItem
			{
				Id = Convert.ToString(r.Id),
				Text = r.Name,
				IsChecked = Nti?.Reasons?.Any(wl_r => wl_r.Id == r.Id) ?? false
			});
		}

		private NtiModel PopulateNtiFromRequest(NtiModel ntiModel)
		{
			if (ntiModel == null)
			{
				throw new ArgumentException(nameof(ntiModel));
			}

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
			var reasons = Request.Form["reason"];
			var status = Request.Form["status"];

			int? statusValue = int.TryParse(status.FirstOrDefault(), out int result) ? result : null;

			var nti = new NtiModel()
			{
				CaseUrn = CaseUrn,
				Reasons = reasons.Select(r => new NtiReasonModel { Id = int.Parse(r) }).ToArray(),
				Status = (NtiStatus?)statusValue,
				Conditions = Array.Empty<NtiConditionModel>(),
				Notes = Notes.Text.StringContents,
				DateStarted = DateIssued.Date.ToDateTime(),
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};

			return nti;
		}

		private async Task LoadWarningLetterFromCache()
		{
			Nti = await _ntiModelService.GetNtiAsync(ContinuationId);
		}

		private async Task LoadWarningLetterFromDB()
		{
			Nti  = await _ntiModelService.GetNtiByIdAsync(NtiId.Value);
		}
	}
}