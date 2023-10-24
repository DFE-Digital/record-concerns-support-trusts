using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : EditNtiUnderConsiderationBaseModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ILogger<AddPageModel> _logger;


		public const int NotesMaxLength = 2000;
		public List<RadioItem> NTIReasonsToConsider;

		[BindProperty(SupportsGet = true, Name = "Urn")] 
		public long CaseUrn { get;  set; }
		
		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }


		public AddPageModel(
			INtiUnderConsiderationModelService ntiModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public IActionResult OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				LoadPageComponents();
				NTIReasonsToConsider = GetReasons().ToList();
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
					NTIReasonsToConsider = GetReasons(data).ToList();
					return Page();
				}
				var newNti = PopulateNtiFromRequest();
				await _ntiModelService.CreateNti(newNti);
				return Redirect($"/case/{CaseUrn}/management");
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
			var nti = new NtiUnderConsiderationModel() { CaseUrn = CaseUrn };
			nti.NtiReasonsForConsidering = reasons.Select(r => (NtiUnderConsiderationReason)int.Parse(r)).ToArray();
			nti.Notes = Notes.Text.StringContents;
			return nti;
		}
		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent(nameof(Notes), Notes?.Text.StringContents);
		}
	}
}