using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Service.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class LiftPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;

		private readonly ILogger<LiftPageModel> _logger;

		public int NotesMaxLength => 2000;

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
			_logger.LogInformation($"{nameof(LiftPageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				long ntiId = 0;

				(_, ntiId) = GetRouteData();

				NtiModel = await _ntiModelService.GetNtiByIdAsync(ntiId);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(LiftPageModel)}::{LoggingHelpers.EchoCallerName()}");

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
			_logger.LogInformation($"{nameof(LiftPageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				long caseUrn = 0;
				long ntiId = 0;

				(caseUrn, ntiId) = GetRouteData();

				var submissionDecisionId = Request.Form["submission-decision-id"].ToString();
				var dtr_day = Request.Form["dtr-day"];
				var dtr_month = Request.Form["dtr-month"];
				var dtr_year = Request.Form["dtr-year"];
				var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
				var notes = Request.Form["nti-notes"].ToString();
				DateTime? dateLifted = null;

				//How the string will appear if no dates were ented 
				if (dtString != "--")
				{
					if (!DateTimeHelper.TryParseExact(dtString, out DateTime parsedDate))
					{
						throw new Exception($"{dtString} was unable to be parsed as a date");
					}

					dateLifted = parsedDate;
				}


				if (!string.IsNullOrEmpty(notes))
				{
					if (notes.Length > NotesMaxLength)
					{
						throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
					}
				}

				var ntiModel = await _ntiModelService.GetNtiByIdAsync(ntiId);
				ntiModel.Notes = notes;
				ntiModel.SumissionDecisionId = submissionDecisionId;
				ntiModel.DateNTILifted = dateLifted;
				ntiModel.ClosedStatusId = (int)NTIStatus.Lifted;
				ntiModel.ClosedAt = DateTime.Now;

				await _ntiModelService.PatchNtiAsync(ntiModel);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(LiftPageModel)}::{LoggingHelpers.EchoCallerName()}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
	}
}