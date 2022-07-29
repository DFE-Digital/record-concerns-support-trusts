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

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService;
		private readonly INtiWarningLetterReasonsCachedService _ntiWarningLetterReasonsCachedService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
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

		public NtiWarningLetterModel WarningLetter { get; set; }

		public long CaseUrn { get; private set; }

		public AddPageModel(INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_ntiWarningLetterReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-WL::AddPageModel::OnGetAsync");

			try
			{
				if(!IsReturningFromConditions) // this is a fresh request, not a return from the conditions page.
				{
					ContinuationId = string.Empty;
				}

				ExtractCaseUrnFromRoute();
				if (!string.IsNullOrWhiteSpace(ContinuationId) && ContinuationId.StartsWith(CaseUrn.ToString()))
				{
					await LoadWarningLetterFromCache();
				}

				Statuses = await GetStatuses();
				Reasons = await GetReasons();

				TempData.Keep(nameof(ContinuationId));
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-WL::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			try
			{
				ExtractCaseUrnFromRoute();

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
				_logger.LogError("Case::NTI-WL::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private async Task<RedirectResult> HandleContinue()
		{
			var ntiModel = await GetUpToDateModel();

			await _ntiWarningLetterModelService.CreateNtiWarningLetter(ntiModel);

			TempData.Remove(nameof(ContinuationId));
			return Redirect($"/case/{CaseUrn}/management");
		}

		private async Task<RedirectResult> HandOverToConditions()
		{
			var ntiModel = await GetUpToDateModel();
			if(string.IsNullOrWhiteSpace(ContinuationId) || !ContinuationId.StartsWith(CaseUrn.ToString()))
			{
				ContinuationId = $"{CaseUrn}_{Guid.NewGuid()}";
			}

			await _ntiWarningLetterModelService.StoreWarningLetterToCache(ntiModel, ContinuationId);

			TempData.Keep(nameof(ContinuationId));
			return Redirect($"/case/{CaseUrn}/management/action/NtiWarningLetter/conditions");
		}

		private async Task<NtiWarningLetterModel> GetUpToDateModel()
		{
			NtiWarningLetterModel nti = null;

			if (!string.IsNullOrWhiteSpace(ContinuationId) && ContinuationId.StartsWith(CaseUrn.ToString())) // conditions have been recorded
			{
				nti = await _ntiWarningLetterModelService.GetWarningLetterFromCache(ContinuationId);
				nti = PopulateNtiFromRequest(nti); // populate current form values on top of values recorded in conditions form
			}

			return nti ?? PopulateNtiFromRequest();
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

		private async Task<IEnumerable<RadioItem>> GetStatuses()
		{
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();
			return statuses.Select(s => new RadioItem
			{
				Id = Convert.ToString(s.Id),
				Text = s.Name,
				IsChecked = WarningLetter?.Status?.Id == s.Id
			});
		}

		private async Task<IEnumerable<RadioItem>> GetReasons()
		{
			var reasons = await _ntiWarningLetterReasonsCachedService.GetAllReasonsAsync();
			return reasons.Select(r => new RadioItem
			{
				Id = Convert.ToString(r.Id),
				Text = r.Name,
				IsChecked = WarningLetter?.Reasons?.Any(wl_r => wl_r.Id == r.Id) ?? false
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
			var status = Request.Form["status"];
			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
			var sentDate = DateTimeHelper.TryParseExact(dtString, out DateTime parsed) ? parsed : (DateTime?)null;

			var notes = Convert.ToString(Request.Form["nti-notes"]);
			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}

			var nti = new NtiWarningLetterModel()
			{
				CaseUrn = CaseUrn,
				Reasons = reasons.Select(r => new NtiWarningLetterReasonModel { Id = int.Parse(r) }).ToArray(),
				Status = int.TryParse(status, out int statusId) ? new NtiWarningLetterStatusModel { Id = statusId } : null,
				Conditions = Array.Empty<NtiWarningLetterConditionModel>(),
				Notes = notes,
				SentDate = sentDate,
				CreatedAt = DateTime.Now.Date,
				UpdatedAt = DateTime.Now.Date
			};

			return nti;
		}

		private async Task LoadWarningLetterFromCache()
		{
			WarningLetter = await _ntiWarningLetterModelService.GetWarningLetterFromCache(ContinuationId);
		}

	}
}