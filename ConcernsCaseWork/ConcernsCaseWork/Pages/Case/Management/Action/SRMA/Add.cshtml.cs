using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CaseActionModels = ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Srma
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		public int NotesMaxLength => 500;
		public IEnumerable<RadioItem> SRMAStatuses => getStatuses();

		public AddPageModel(
			ISRMAService srmaModelService, ILogger<AddPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::AddPageModel::OnGetAsync");
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				ValidateSRMA();

				var caseUrn = GetRouteData();
				var srma = CreateSRMA(caseUrn);
				await _srmaModelService.SaveSRMA(srma);

				return Redirect($"/case/{srma.CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
		}

		private IEnumerable<RadioItem> getStatuses()
		{
			var statuses = (SRMAStatus[])Enum.GetValues(typeof(SRMAStatus));
			return statuses.Where(s => s != SRMAStatus.Unknown)
						   .Select(s => new RadioItem
						   {
							   Id = s.ToString(),
							   Text = EnumHelper.GetEnumDescription(s)
						   });
		}

		private long GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			return caseUrn;
		}

		private void ValidateSRMA()
		{
			var status = Request.Form["status"];

			if (string.IsNullOrEmpty(status))
			{
				throw new Exception("SRMA status not selected");
			}

			if (!Enum.TryParse<SRMAStatus>(status, ignoreCase: true, out SRMAStatus srmaStatus))
			{
				throw new Exception($"Can't parse SRMA status {srmaStatus}");
			}

			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];

			var allowedFormats = new string[] { "dd-MM-yyyy", "d-M-yyyy" };

			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";

			if (!DateTime.TryParseExact(dtString, allowedFormats, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out DateTime dateOffered))
			{
				throw new Exception($"SRMA offered date is not valid {dtString}");
			}

			var srma_notes = Request.Form["srma-notes"];

			if (!string.IsNullOrEmpty(srma_notes))
			{
				var notes = srma_notes.ToString();
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}
		}

		private CaseActionModels.SRMAModel CreateSRMA(long caseUrn)
		{
			var srma = new CaseActionModels.SRMAModel();

			srma.CaseUrn = caseUrn;

			var status = Request.Form["status"];

			srma.Status = Enum.Parse<SRMAStatus>(status);

			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";

			var allowedFormats = new string[] { "dd-MM-yyyy", "d-M-yyyy" };
			srma.DateOffered = DateTime.ParseExact(dtString, allowedFormats, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None);

			var notes = Request.Form["srma-notes"].ToString();

			srma.Notes = notes;


			return srma;
		}
	}
}