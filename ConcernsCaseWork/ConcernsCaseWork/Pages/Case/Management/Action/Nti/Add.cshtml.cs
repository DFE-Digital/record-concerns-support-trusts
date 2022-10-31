
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Service.Redis.Nti;
using ConcernsCaseWork.Services.Nti;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiStatusesCachedService _ntiStatusesCachedService;
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

		public NtiModel Nti { get; set; }

		public long CaseUrn { get; private set; }

		public long? NtiId { get; set; }

		public string CancelLinkUrl { get; set; }

		public AddPageModel(INtiStatusesCachedService ntiWarningLetterStatusesCachedService,
			INtiReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiModelService ntiWarningLetterModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_ntiReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiModelService = ntiWarningLetterModelService;
			_logger = logger;
		}


		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI::AddPageModel::OnGetAsync");

			try
			{
				if (!IsReturningFromConditions) // this is a fresh request, not a return from the conditions page.
				{
					ContinuationId = string.Empty;
				}

				ExtractCaseUrnFromRoute();
				ExtractNtiIdFromRoute();

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

				Statuses = await GetStatuses();
				Reasons = await GetReasons();
			
				CancelLinkUrl = NtiId.HasValue ? @$"/case/{CaseUrn}/management/action/nti/{NtiId.Value}" 
														 : @$"/case/{CaseUrn}/management/action";

				TempData.Keep(nameof(ContinuationId));
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			try
			{
				ExtractCaseUrnFromRoute();
				ExtractNtiIdFromRoute();

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
				_logger.LogError("Case::NTI::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

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

		private void ExtractCaseUrnFromRoute()
		{
			if (TryGetRouteValueInt64("urn", out var caseUrn))
			{
				CaseUrn = caseUrn;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private void ExtractNtiIdFromRoute()
		{
			NtiId = TryGetRouteValueInt64("NtiId", out var ntiId) ? (long?)ntiId : null;
		}

		private async Task<IEnumerable<RadioItem>> GetStatuses()
		{
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();
			return statuses.Where(s => !s.IsClosingState).Select(s => new RadioItem
			{
				Id = Convert.ToString(s.Id),
				Text = s.Name,
				IsChecked = Nti?.Status?.Id == s.Id
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
			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
			var dateStarted = DateTimeHelper.TryParseExact(dtString, out DateTime parsed) ? parsed : (DateTime?)null;

			var notes = Convert.ToString(Request.Form["nti-notes"]);
			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}

			var nti = new NtiModel()
			{
				CaseUrn = CaseUrn,
				Reasons = reasons.Select(r => new NtiReasonModel { Id = int.Parse(r) }).ToArray(),
				Status = int.TryParse(status, out int statusId) ? new NtiStatusModel { Id = statusId } : null,
				Conditions = Array.Empty<NtiConditionModel>(),
				Notes = notes,
				DateStarted = dateStarted,
				CreatedAt = DateTime.Now.Date,
				UpdatedAt = DateTime.Now.Date
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