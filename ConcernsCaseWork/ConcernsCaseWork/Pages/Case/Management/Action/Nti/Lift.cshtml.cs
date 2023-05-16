using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
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
	public class LiftPageModel : CloseNtiBasePage
	{
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<LiftPageModel> _logger;

		public NtiModel NtiModel { get; set; }

		public LiftPageModel(
			INtiModelService ntiModelService,
			ILogger<LiftPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				NtiModel = await _ntiModelService.GetNtiByIdAsync(NtiId);

				if (NtiModel.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}");
				}

				LoadPageComponents(NtiModel);

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
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid) 
				{
					ResetLiftPageComponentsOnValidationError();
					return Page();
				}

				var ntiModel = await _ntiModelService.GetNtiByIdAsync(NtiId);

				ntiModel.Notes = Notes.Text.StringContents;
				ntiModel.SumissionDecisionId = DecisionID.Text.StringContents;
				ntiModel.DateNTILifted = !DateNTILifted.Date?.IsEmpty() ?? false ? DateNTILifted.Date?.ToDateTime() : null;
				ntiModel.ClosedStatusId = (int)NTIStatus.Lifted;
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

		private void LoadPageComponents(NtiModel nti)
		{
			Notes.Text.StringContents = nti.Notes;
		}

	}
}