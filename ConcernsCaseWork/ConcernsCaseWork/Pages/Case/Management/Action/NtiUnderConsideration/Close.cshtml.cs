using ConcernsCaseWork.API.Contracts.Constants;
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
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;
		private readonly ILogger<ClosePageModel> _logger;

		
		public IEnumerable<RadioItem> NTIStatuses;
		private static int _max;

		public long CaseUrn { get; private set; }
		public NtiUnderConsiderationModel NtiModel { get; set; }

		[BindProperty] 
		public TextAreaUiComponent Notes { get; set; }//= BuildNotesComponent();

		public ClosePageModel(
			INtiUnderConsiderationModelService ntiModelService,
			INtiUnderConsiderationStatusesCachedService ntiStatusesCachedService,
			ILogger<ClosePageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
			_logger = logger;
			_max = NtiConstants.MaxNotesLength;
		}
		

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::ClosePageModel::OnGetAsync");

			try
			{
				CaseUrn = ExtractCaseUrnFromRoute();
				var ntiUcId = ExtractNtiUcIdFromRoute();
				NtiModel = await _ntiModelService.GetNtiUnderConsideration(ntiUcId);
				if (NtiModel.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/ntiunderconsideration/{ntiUcId}");
				}
				LoadPageComponents();
				Notes.Text.StringContents = NtiModel.Notes;
				NTIStatuses = await GetStatusesForUiAsync();
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
				var status = Request.Form["status"];
				var success = int.TryParse(status, out var result);
				if (!success)
				{
					ModelState.AddModelError("options", "Please select a reason for closing NTI under consideration");	
				}
				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					NtiModel =PopulateNtiFromRequest();
					Notes.Text.StringContents = NtiModel.Notes;
					NTIStatuses = await GetStatusesForUiAsync();
					return Page();
				}
				CaseUrn = ExtractCaseUrnFromRoute();
				var ntiUcId = ExtractNtiUcIdFromRoute();
				var freshNti = await _ntiModelService.GetNtiUnderConsideration(ntiUcId);
				var ntiWithUpdatedValues = PopulateNtiFromRequest();
				freshNti.Notes = ntiWithUpdatedValues.Notes;
				freshNti.ClosedStatusId = ntiWithUpdatedValues.ClosedStatusId;
				freshNti.ClosedAt = DateTime.Now; // Note that we should probably be using UTC? Keeping this consistent with other areas which mostly use DateTime.Now. 
				await _ntiModelService.PatchNtiUnderConsideration(freshNti);
				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["NTI-UC.Message"] = ex.Message;

				var ntiUcId = ExtractNtiUcIdFromRoute();
				NtiModel = await _ntiModelService.GetNtiUnderConsideration(ntiUcId);
				NTIStatuses = await GetStatusesForUiAsync();
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
				throw new Exception("CaseUrn not found in the route");
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
				throw new Exception("NtiUcId not found in the route");
			}
		}

		private async Task<IEnumerable<RadioItem>> GetStatusesForUiAsync()
		{
			var statuses = await _ntiStatusesCachedService.GetAllStatuses();
			return statuses.Select(s => new RadioItem
			{
				Id = Convert.ToString(s.Id), Text = s.Name, IsChecked = false // all statuses are unchecked when close page is opened
			});
		}

		private NtiUnderConsiderationModel PopulateNtiFromRequest()
		{
			var statusId = GetRequestedStatusId();
			var notes =Notes.Text.StringContents;
			var id = ExtractNtiUcIdFromRoute();
			var nti = new NtiUnderConsiderationModel() { Id = id, CaseUrn = CaseUrn, Notes = notes, ClosedStatusId = statusId };
			return nti;
		}

		private int GetRequestedStatusId()
		{
			var status = Request.Form["status"];
			return int.TryParse(status, out var statusId)
				? statusId
				: throw new InvalidOperationException($"Please select a reason for closing NTI under consideration");
		}
		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent();
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("nti-notes", nameof(Notes), "Notes (optional)")
			{
				HintText = "Case owners can record any information they want that feels relevant to the action",
				Text = new ValidateableString() { MaxLength = _max, StringContents = contents, DisplayName = "Notes" }
			};
		
		private void ResetOnValidationError()
		{
			
			Notes = BuildNotesComponent(Notes.Text.StringContents);
		}


	}
}