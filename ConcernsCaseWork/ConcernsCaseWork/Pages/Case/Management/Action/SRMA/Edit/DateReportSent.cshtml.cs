using ConcernsCaseWork.Helpers;
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
	public class EditDateReportSentPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateReportSentPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }

		public EditDateReportSentPageModel(ISRMAService srmaModelService, ILogger<EditDateReportSentPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::Action::SRMA::EditDateReportSentPageModel::OnGetAsync");

				(long caseUrn, long srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);
				
				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}/closed");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateReportSentPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;

			try
			{
				_logger.LogInformation("Case::Action::SRMA::EditDateReportSentPageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				var dtr_day = Request.Form["dtr-day"].ToString();
				var dtr_month = Request.Form["dtr-month"].ToString();
				var dtr_year = Request.Form["dtr-year"].ToString();
				DateTime? reportSentDate = null;

				var dtString = string.IsNullOrEmpty(dtr_day) && string.IsNullOrEmpty(dtr_month) && string.IsNullOrEmpty(dtr_year) ?
				String.Empty : $"{dtr_day}-{dtr_month}-{dtr_year}";

				if (!string.IsNullOrEmpty(dtString))
				{
					if (!DateTimeHelper.TryParseExact(dtString, out DateTime dt))
					{
						throw new InvalidOperationException($"{dtString} is an invalid date");
					}

					reportSentDate = dt;
				}

				await _srmaModelService.SetDateReportSent(srmaId, reportSentDate);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateReportSentPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private (long caseUrn, long srmaId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["caseUrn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			return (caseUrn, srmaId);
		}
	
	}
}