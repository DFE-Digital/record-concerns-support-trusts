using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;

		private readonly ILogger<ClosePageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiWLId")]
		public long WarningLetterId { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent NtiWarningLetterStatus { get; set; } = BuildStatusComponent();

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

		public ClosePageModel(
			INtiWarningLetterModelService ntiWarningLetterModelService,
			ILogger<ClosePageModel> logger)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var model = await _ntiWarningLetterModelService.GetNtiWarningLetterId(WarningLetterId);
				
				if (model.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/ntiwarningletter/{WarningLetterId}");
				}

				LoadComponents(model);
				
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
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					return Page();
				}

				var ntiWarningLetter = await _ntiWarningLetterModelService.GetNtiWarningLetterId(WarningLetterId);
				ntiWarningLetter.Notes = Notes.Text.StringContents;
				ntiWarningLetter.ClosedStatusId = NtiWarningLetterStatus.SelectedId;
				ntiWarningLetter.ClosedAt = DateTime.Now;

				await _ntiWarningLetterModelService.PatchNtiWarningLetter(ntiWarningLetter);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadComponents(NtiWarningLetterModel warningLetterModel)
		{
			Notes.Text.StringContents = warningLetterModel.Notes;
		}

		private void ResetOnValidationError()
		{
			NtiWarningLetterStatus = BuildStatusComponent(NtiWarningLetterStatus.SelectedId);
			Notes = BuildNotesComponent(Notes.Text.StringContents);
		}

		private static RadioButtonsUiComponent BuildStatusComponent(int? selectedId = null)
		{
			var enumValues = new List<NtiWarningLetterStatus>()
			{
				API.Contracts.NtiWarningLetter.NtiWarningLetterStatus.CancelWarningLetter,
				API.Contracts.NtiWarningLetter.NtiWarningLetterStatus.ConditionsMet,
				API.Contracts.NtiWarningLetter.NtiWarningLetterStatus.EscalateToNoticeToImprove,
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "status", Name: nameof(NtiWarningLetterStatus), "")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "Status",
				Required = true,
				ErrorTextForRequiredField = "Please select a reason for closing the warning letter"
			};
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("nti-notes", nameof(Notes), "Finalise notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action",
			Text = new ValidateableString()
			{
				MaxLength = 2000,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};
	}
}