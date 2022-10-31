using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;

		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService;
		private readonly ILogger<ClosePageModel> _logger;

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> NTIStatuses;

		public NtiWarningLetterModel NtiWarningLetterModel { get; set; }

		public ClosePageModel(
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			ILogger<ClosePageModel> logger)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-WL::ClosePageModel::OnGetAsync");

			try
			{
				long ntiWLId = 0;
				long caseUrn = 0;

				(caseUrn, ntiWLId) = GetRouteData();

				NtiWarningLetterModel = await _ntiWarningLetterModelService.GetNtiWarningLetterId(ntiWLId);
				
				if (NtiWarningLetterModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/ntiwarningletter/{ntiWLId}");
				}
				
				NTIStatuses = await GetStatusesForUI();

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-WL::ClosePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}

		}

		private (long caseUrn, long ntiWLId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiWLIdValue = RouteData.Values["ntiWLId"];
			if (ntiWLIdValue == null || !long.TryParse(ntiWLIdValue.ToString(), out long ntiWLId) || ntiWLId == 0)
				throw new Exception("ntiWLId is null or invalid to parse");

			return (caseUrn, ntiWLId);
		}

		private async Task<IEnumerable<RadioItem>> GetStatusesForUI()
		{
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			return statuses.Where(s => s.IsClosingState).Select(s => new RadioItem
			{
				Id = Convert.ToString(s.Id),
				Text = s.Name,
				Description = s.Description,
				IsChecked = false   // all statuses are unchecked when close page is opened
			});
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				long caseUrn = 0;
				long ntiWLId = 0;

				(caseUrn, ntiWLId) = GetRouteData();

				ValidateForm();

				var status = Request.Form["status"].ToString();
				var notes = Request.Form["nti-notes"].ToString();
				var statusId = int.Parse(status);

				var ntiWarningLetter = await _ntiWarningLetterModelService.GetNtiWarningLetterId(ntiWLId);
				ntiWarningLetter.Notes = notes;
				ntiWarningLetter.ClosedStatusId = statusId;
				ntiWarningLetter.ClosedAt = DateTime.Now;

				await _ntiWarningLetterModelService.PatchNtiWarningLetter(ntiWarningLetter);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-WL::ClosePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private void ValidateForm()
		{
			var status = Request.Form["status"];
			var notes = Request.Form["nti-notes"].ToString();

			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}

			if (!int.TryParse(status, out var statusId))
			{
				throw new Exception("Closing status Id could not be resolved");
			}
		}
	}
}