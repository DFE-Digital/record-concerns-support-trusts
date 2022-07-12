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

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;
		private readonly ILogger<ClosePageModel> _logger;

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> NTIStatuses;

		public long CaseUrn { get; private set; }
		public NtiModel NtiModel { get; set; }

		public ClosePageModel(
			INtiUnderConsiderationModelService ntiModelService,
			INtiUnderConsiderationStatusesCachedService ntiStatusesCachedService,
			ILogger<ClosePageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::ClosePageModel::OnGetAsync");

			try
			{
				CaseUrn = ExtractCaseUrnFromRoute();

				var ntiUcId = ExtractNtiUcIdFromRoute();
				NtiModel = await _ntiModelService.GetNtiUnderConsideration(ntiUcId);

				NTIStatuses = await GetStatusesForUI();

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::ClosePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}

		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				CaseUrn = ExtractCaseUrnFromRoute();
				var ntiWithUpdatedValues = PopulateNtiFromRequest();

				var freshNti = await _ntiModelService.GetNtiUnderConsideration(ntiWithUpdatedValues.Id);
				freshNti.Notes = ntiWithUpdatedValues.Notes;
				freshNti.ClosedStatusId = ntiWithUpdatedValues.ClosedStatusId;
				freshNti.ClosedAt = freshNti.UpdatedAt;

				await _ntiModelService.PatchNtiUnderConsideration(freshNti);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::ClosePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private long ExtractCaseUrnFromRoute()
		{
			if (TryGetRouteValueInt64("urn", out var caseUrn))
			{
				return caseUrn;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private long ExtractNtiUcIdFromRoute()
		{
			if (TryGetRouteValueInt64("ntiUCId", out var ntiUcId))
			{
				return ntiUcId;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private async Task<IEnumerable<RadioItem>> GetStatusesForUI()
		{
			var statuses = await _ntiStatusesCachedService.GetAllStatuses();
			return statuses.Select(s => new RadioItem
			{
				Id = Convert.ToString(s.Id),
				Text = s.Name,
				IsChecked = false   // all statuses are unchecked when close page is opened
			});
		}

		private NtiModel PopulateNtiFromRequest()
		{
			var status = Request.Form["status"];
			var notes = Convert.ToString(Request.Form["nti-notes"]);

			var nti = new NtiModel()
			{
				Id = ExtractNtiUcIdFromRoute(),
				CaseUrn = CaseUrn,
			};

			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
				else
				{
					nti.Notes = notes;
				}
			}

			if (int.TryParse(status, out var statusId))
			{
				nti.ClosedStatusId = statusId;
			}
			else
			{
				throw new Exception("Closing status Id could not be resolved");
			}

			return nti;
		}
	}
}