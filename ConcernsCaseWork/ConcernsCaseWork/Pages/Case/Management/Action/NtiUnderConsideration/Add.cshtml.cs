using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsider => GetReasons();

		public long CaseUrn { get; private set; }

		public AddPageModel(
			ISRMAService srmaModelService, ILogger<AddPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::AddPageModel::OnGetAsync");

			try
			{
				CaseUrn = GetRouteValueInt64("urn");
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var caseUrn = GetRouteData();

				ValidateSRMA();

				var srma = CreateSRMA(caseUrn);
				await _srmaModelService.SaveSRMA(srma);

				return Redirect($"/case/{srma.CaseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private IEnumerable<RadioItem> GetReasons()
		{
			var statuses = (NtiReasonForConsidering[])Enum.GetValues(typeof(NtiReasonForConsidering));
			return statuses.Where(r => r != NtiReasonForConsidering.None)
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
			var status = Request.Form["reason"];

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

			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";

			if (!DateTimeHelper.TryParseExact(dtString, out DateTime dateOffered))
			{
				throw new InvalidOperationException($"SRMA offered date is not valid {dtString}");
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

		private SRMAModel CreateSRMA(long caseUrn)
		{
			var status = Request.Form["status"];
			var notes = Request.Form["srma-notes"].ToString();
			var dtr_day = Request.Form["dtr-day"];
			var dtr_month = Request.Form["dtr-month"];
			var dtr_year = Request.Form["dtr-year"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
			var dateOffered = DateTimeHelper.ParseExact(dtString);

			var srma = new SRMAModel(
				0,
				caseUrn,
				dateOffered,
				null,
				null,
				null,
				null,
				Enum.Parse<SRMAStatus>(status),
				notes,
				SRMAReasonOffered.Unknown,
				DateTime.Now
				);

			return srma;
		}
	}
}