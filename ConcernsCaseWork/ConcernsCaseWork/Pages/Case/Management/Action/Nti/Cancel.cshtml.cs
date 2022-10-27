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
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Enums;
using Service.TRAMS.Helpers;
using Service.TRAMS.Nti;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class CancelPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;

		private readonly ILogger<CancelPageModel> _logger;

		public int NotesMaxLength => 2000;

		public NtiModel NtiModel { get; set; }

		public CancelPageModel(
			INtiModelService ntiModelService,
			ILogger<CancelPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation($"{nameof(CancelPageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				long ntiId = 0;
				long caseUrn = 0;

				(caseUrn, ntiId) = GetRouteData();

				NtiModel = await _ntiModelService.GetNtiByIdAsync(ntiId);
					
				if (NtiModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/nti/{ntiId}");
				}

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(CancelPageModel)}::{LoggingHelpers.EchoCallerName()}");

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}

		}

		private (long caseUrn, long ntiId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiIdValue = RouteData.Values["ntiId"];
			if (ntiIdValue == null || !long.TryParse(ntiIdValue.ToString(), out long ntiId) || ntiId == 0)
				throw new Exception("ntiId is null or invalid to parse");

			return (caseUrn, ntiId);
		}




		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogInformation($"{nameof(CancelPageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				long caseUrn = 0;
				long ntiId = 0;

				(caseUrn, ntiId) = GetRouteData();

				var notes = Request.Form["nti-notes"].ToString();

				if (!string.IsNullOrEmpty(notes))
				{
					if (notes.Length > NotesMaxLength)
					{
						throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
					}
				}

				var ntiModel = await _ntiModelService.GetNtiByIdAsync(ntiId);
				ntiModel.Notes = notes;
				ntiModel.ClosedStatusId = (int)NTIStatus.Cancelled;
				ntiModel.ClosedAt = DateTime.Now;

				await _ntiModelService.PatchNtiAsync(ntiModel);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(CancelPageModel)}::{LoggingHelpers.EchoCallerName()}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

	}
}