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
using Service.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.Nti;
using Service.TRAMS.Helpers;
using ConcernsCaseWork.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<ClosePageModel> _logger;

		public int NotesMaxLength => 2000;

		public NtiModel Nti { get; set; }

		public ClosePageModel(
			INtiModelService ntiModelService,
			ILogger<ClosePageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation($"{nameof(ClosePageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				long ntiId = 0;

				(_, ntiId) = GetRouteData();

				Nti = await _ntiModelService.GetNtiByIdAsync(ntiId);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Case::NTI::ClosePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}

		}

		private (long caseUrn, long ntiId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiWLIdValue = RouteData.Values["ntiId"];
			if (ntiWLIdValue == null || !long.TryParse(ntiWLIdValue.ToString(), out long ntiId) || ntiId == 0)
				throw new Exception("ntiId is null or invalid to parse");

			return (caseUrn, ntiId);
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				long caseUrn = 0;
				long ntiId = 0;

				(caseUrn, ntiId) = GetRouteData();

				ValidateForm();

				var nti = await _ntiModelService.GetNtiByIdAsync(ntiId);
				var updated = UpdateNti(nti);
				await _ntiModelService.PatchNtiAsync(nti);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-WL::ClosePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private NtiModel UpdateNti(NtiModel nti)
		{
			var notes = Request.Form["nti-notes"].ToString();

			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
			var date = DateTimeHelper.TryParseExact(dtString, out DateTime parsed) ? parsed : (DateTime?)null;

			nti.Notes = notes;
			nti.ClosedAt = date;

			return nti;
		}

		private void ValidateForm()
		{
			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];

			if (!AreAllEmpty(dtr_day, dtr_month, dtr_year))
			{
				var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
				if (!DateTimeHelper.TryParseExact(dtString, out _))
				{
					throw new Exception("Date provided is invalid.");
				}
			}

			var notes = Request.Form["nti-notes"].ToString();
			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}
		}

		private bool AreAllEmpty(params string[] args)
		{
			return args.All(s => string.IsNullOrEmpty(s));
		}
	}
}