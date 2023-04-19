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
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ILogger<EditPageModel> _logger;
		
		public const int NotesMaxLength = 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsiderForUI;
		
		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		public long CaseUrn { get; private set; }
		public NtiUnderConsiderationModel NtiModel { get; set; }

		public EditPageModel(
			INtiUnderConsiderationModelService ntiModelService,
			ILogger<EditPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::EditPageModel::OnGetAsync");

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
				NTIReasonsToConsiderForUI = GetReasonsForUI(NtiModel);
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				
				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					return Page();
				}
				
				CaseUrn = ExtractCaseUrnFromRoute();
				var ntiWithUpdatedValues = PopulateNtiFromRequest();
				var freshFromDb = await _ntiModelService.GetNtiUnderConsideration(ntiWithUpdatedValues.Id); // this db call is necessary as the API is only designed to simply patch the whole nti
				freshFromDb.NtiReasonsForConsidering = ntiWithUpdatedValues.NtiReasonsForConsidering;
				freshFromDb.Notes = ntiWithUpdatedValues.Notes;
				
				var updated = await _ntiModelService.PatchNtiUnderConsideration(freshFromDb);

				return Redirect($"/case/{CaseUrn}/management/action/ntiunderconsideration/{updated.Id}");
			}
			catch (InvalidUserInputException ex)
			{
				TempData["NTI-UC.Message"] = ex.Message;
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::EditPageModel::OnPostAsync::Exception - {Message}", ex.Message);

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
				throw new Exception("CaseUrn not found in the route");
			}
		}

		private IEnumerable<RadioItem> GetReasonsForUI(NtiUnderConsiderationModel ntiModel)
		{
			var reasonValues = Enum.GetValues<NtiUnderConsiderationReason>().ToList();

			return reasonValues.Select(r => new RadioItem
			{
				Id = Convert.ToString((int)r),
				Text = EnumHelper.GetEnumDescription(r),
				IsChecked = ntiModel?.NtiReasonsForConsidering?.Any(ntiR => ntiR.Id == (int)r) == true,
			});
		}

		private NtiUnderConsiderationModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			
			var nti = new NtiUnderConsiderationModel() { 
				Id = ExtractNtiUcIdFromRoute(),
				CaseUrn = CaseUrn,
				Notes =  Notes.Text.StringContents
			};
			nti.NtiReasonsForConsidering = reasons.Select(r => new NtiReasonForConsideringModel { Id = int.Parse(r) }).ToArray();
			return nti;
		}
		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent();
		}
		
		private TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("nti-notes", nameof(Notes), "Notes (optional)")
			{
				HintText = "Case owners can record any information they want that feels relevant to the action",
				Text = new ValidateableString()
				{
					MaxLength = NotesMaxLength,
					StringContents = contents,
					DisplayName = "Notes"
				}
			};
		
		private void ResetOnValidationError()
		{
			
			Notes = BuildNotesComponent(Notes.Text.StringContents);
		}

	}
}