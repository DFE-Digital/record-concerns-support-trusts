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
using ConcernsCaseWork.Logging;
using Microsoft.Graph.Models;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : EditNtiUnderConsiderationBaseModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ILogger<EditPageModel> _logger;
		
		public const int NotesMaxLength = 2000;
		public List<RadioItem> NTIReasonsToConsiderForUI;
		
		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }
		
		[BindProperty(SupportsGet = true, Name = "Urn")] 
		public long CaseUrn { get;  set; }

		[BindProperty(SupportsGet = true, Name = "ntiUCId")]
		public long NtiId { get; set; }

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
			_logger.LogMethodEntered();

			try
			{
				NtiModel = await _ntiModelService.GetNtiUnderConsideration(NtiId);
				if (NtiModel.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/ntiunderconsideration/{NtiId}");
				}
				LoadPageComponents();
				Notes.Text.StringContents = NtiModel.Notes;
				NTIReasonsToConsiderForUI = GetReasons(NtiModel).ToList();
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
			try
			{
				if (!ModelState.IsValid)
				{
					LoadPageComponents();
					var data = PopulateNtiFromRequest();
					NTIReasonsToConsiderForUI = GetReasons(NtiModel).ToList();
					return Page();
				}
				
				var ntiWithUpdatedValues = PopulateNtiFromRequest();
				var freshFromDb = await _ntiModelService.GetNtiUnderConsideration(ntiWithUpdatedValues.Id); // this db call is necessary as the API is only designed to simply patch the whole nti
				freshFromDb.NtiReasonsForConsidering = ntiWithUpdatedValues.NtiReasonsForConsidering;
				freshFromDb.Notes = ntiWithUpdatedValues.Notes;
				var updated = await _ntiModelService.PatchNtiUnderConsideration(freshFromDb);
				return Redirect($"/case/{CaseUrn}/management/action/ntiunderconsideration/{updated.Id}");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private NtiUnderConsiderationModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			
			var nti = new NtiUnderConsiderationModel() { 
				Id = NtiId,
				CaseUrn = CaseUrn,
				Notes =  Notes.Text.StringContents
			};
			nti.NtiReasonsForConsidering = reasons.Select(r => (NtiUnderConsiderationReason)int.Parse(r)).ToArray();
			return nti;
		}
		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent(nameof(Notes), Notes?.Text.StringContents);
		}
	}
}