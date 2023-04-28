using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
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
		private static int _max;

		[BindProperty(SupportsGet = true, Name = "Urn")] 
		public long CaseUrn { get;  set; }
		
		[BindProperty(SupportsGet = true, Name = "ntiUCId")] 
		public int NtiId { get; set; }
		public NtiUnderConsiderationModel NtiModel { get; set; }

		[BindProperty] 
		public TextAreaUiComponent Notes { get; set; }
		
		[BindProperty]
		public RadioButtonsUiComponent NTIClosedStatus { get; set; }

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
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
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
				var freshNti = await _ntiModelService.GetNtiUnderConsideration(NtiId);
				freshNti.Notes =Notes.Text.StringContents;
				freshNti.ClosedStatusId = NTIClosedStatus.SelectedId;
				freshNti.ClosedAt = DateTime.Now; 
				await _ntiModelService.PatchNtiUnderConsideration(freshNti);
				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent();
			NTIClosedStatus = BuildStatusComponent();
		}

		
		private void ResetOnValidationError()
		{
			
			Notes = BuildNotesComponent(Notes.Text.StringContents);
			NTIClosedStatus = BuildStatusComponent(NTIClosedStatus.SelectedId);
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("nti-notes", nameof(Notes), "Notes (optional)")
			{
				HintText = "Case owners can record any information they want that feels relevant to the action",
				Text = new ValidateableString() { MaxLength = _max, StringContents = contents, DisplayName = "Notes" }
			};

		private static RadioButtonsUiComponent BuildStatusComponent(int? selectedId = null)
		{
			var enumValues = new List<NtiClosedStatus>()
			{
				Enums.NtiClosedStatus.ToBeEscalated,
				Enums.NtiClosedStatus.NoFurtherAction
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "nti-status", Name: nameof(NtiClosedStatus), "What is the status of the NTI?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "Status",
				Required = true,
				ErrorTextForRequiredField = "Please select a reason for closing NTI under consideration"
			};
		}
	}
}