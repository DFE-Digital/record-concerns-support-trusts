using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ResolvePageModel : AbstractPageModel
	{
		private readonly ILogger<ResolvePageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		public SrmaCloseTextModel CloseTextModel { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

		[BindProperty]
		public CheckboxUiComponent Confirm { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty(SupportsGet = true, Name = "resolution")] public string Resolution { get; set; }

		public ResolvePageModel(
			ISRMAService srmaModelService, ILogger<ResolvePageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var model = await _srmaModelService.GetSRMAById(SrmaId);

				if (model.IsClosed)
				{
					return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}");
				}

				SetupPage(Resolution);
				LoadPageComponents(model);
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
				SetupPage(Resolution);

				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					return Page();
				}

				SRMAStatus resolvedStatus = GetStatus();

				await _srmaModelService.SetNotes(SrmaId, Notes.Text.StringContents);
				await _srmaModelService.SetStatus(SrmaId, resolvedStatus);
				await _srmaModelService.SetDateClosed(SrmaId);

				return Redirect($"/case/{CaseId}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private SRMAStatus GetStatus()
		{
			SRMAStatus resolvedStatus;

			switch (Resolution)
			{
				case SrmaConstants.ResolutionComplete:
					resolvedStatus = SRMAStatus.Complete;
					break;
				case SrmaConstants.ResolutionCancelled:
					resolvedStatus = SRMAStatus.Cancelled;
					break;
				case SrmaConstants.ResolutionDeclined:
					resolvedStatus = SRMAStatus.Declined;
					break;
				default:
					throw new Exception("resolution value is null or invalid to parse");
			}

			return resolvedStatus;
		}

		private void LoadPageComponents(SRMAModel model)
		{
			Notes.Text.StringContents = model.Notes;
			Confirm = BuildConfirmComponent(CloseTextModel);
		}

		private void ResetOnValidationError()
		{
			Notes = BuildNotesComponent(Notes.Text.StringContents);
			Confirm = BuildConfirmComponent(CloseTextModel, Confirm.Checked);
		}

		private void SetupPage(string resolution)
		{
			CloseTextModel = CaseActionsMapping.ToSrmaCloseText(resolution);
			ViewData[ViewDataConstants.Title] = CloseTextModel.Title;
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("srma-notes", nameof(Notes), "Finalise notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action",
			Text = new ValidateableString()
			{
				MaxLength = SrmaConstants.NotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};

		private static CheckboxUiComponent BuildConfirmComponent (SrmaCloseTextModel closedTextModel, bool confirmed = false)
		{
			return new CheckboxUiComponent("srma-confirm-check", nameof(Confirm), "")
			{
				Checked = confirmed,
				Required = true,
				Text = closedTextModel.ConfirmText,
				ErrorTextForRequiredField = closedTextModel.ConfirmText
			};
		}
	}
}