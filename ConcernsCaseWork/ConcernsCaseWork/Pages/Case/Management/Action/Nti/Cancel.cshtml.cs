using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Nti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class CancelPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;

		private readonly ILogger<CancelPageModel> _logger;

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "NtiId")]
		public long NtiId { get; set; }

		public CancelPageModel(
			INtiModelService ntiModelService,
			ILogger<CancelPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var model = await _ntiModelService.GetNtiByIdAsync(NtiId);

				if (model.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}");
				}

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
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					LoadPageComponents();
					return Page();
				}

				var ntiModel = await _ntiModelService.GetNtiByIdAsync(NtiId);
				ntiModel.Notes = Notes.Text.StringContents;
				ntiModel.ClosedStatusId = NtiStatus.Cancelled;
				ntiModel.ClosedAt = DateTime.Now;

				await _ntiModelService.PatchNtiAsync(ntiModel);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(NtiModel model)
		{
			LoadPageComponents();

			Notes.Text.StringContents = model.Notes;
		}

		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent(Notes?.Text.StringContents);
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("nti-notes", nameof(Notes), "Finalise notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action.",
			Text = new ValidateableString()
			{
				MaxLength = 2000,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};
	}
}