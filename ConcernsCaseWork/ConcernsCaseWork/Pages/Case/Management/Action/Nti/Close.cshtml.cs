﻿using ConcernsCaseWork.API.Contracts.NoticeToImprove;
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
	public class ClosePageModel : CloseNtiBasePage
	{
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<ClosePageModel> _logger;

		public NtiModel NtiModel { get; set; }

		public ClosePageModel(
			INtiModelService ntiModelService,
			ILogger<ClosePageModel> logger)
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
					ResetClosePageComponentsOnValidationError();
					return Page();
				}

				var ntiModel = await _ntiModelService.GetNtiByIdAsync(NtiId);

				ntiModel.Notes = Notes.Text.StringContents;
				ntiModel.DateNTIClosed = !DateNTIClosed.Date?.IsEmpty() ?? false ? DateNTIClosed.Date?.ToDateTime() : null;
				ntiModel.ClosedStatusId = NtiStatus.Closed;
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