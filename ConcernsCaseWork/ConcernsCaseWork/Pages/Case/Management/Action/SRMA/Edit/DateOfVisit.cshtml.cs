using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDateOfVisitPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateOfVisitPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }

		public EditDateOfVisitPageModel(ISRMAService srmaModelService, ILogger<EditDateOfVisitPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;

			try
			{
				_logger.LogInformation("Case::Action::SRMA::EditDateOfVisitPageModel::OnGetAsync");

				(caseUrn, srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateOfVisitPageModel::OnGetAsync::Exception - {Message}", ex.Message);
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
				_logger.LogInformation("Case::Action::SRMA::EditDateOfVisitPageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				var start_dtr_day = Request.Form["start-dtr-day"].ToString();
				var start_dtr_month = Request.Form["start-dtr-month"].ToString();
				var start_dtr_year = Request.Form["start-dtr-year"].ToString();

				var end_dtr_day = Request.Form["end-dtr-day"].ToString();
				var end_dtr_month = Request.Form["end-dtr-month"].ToString();
				var end_dtr_year = Request.Form["end-dtr-year"].ToString();

				var start_dtString = $"{start_dtr_day}-{start_dtr_month}-{start_dtr_year}";
				var end_dtString = string.IsNullOrEmpty(end_dtr_day) && string.IsNullOrEmpty(end_dtr_month) && string.IsNullOrEmpty(end_dtr_year) ? 
					String.Empty : $"{end_dtr_day}-{end_dtr_month}-{end_dtr_year}";


				ValidateDatesOfVisit(start_dtString, end_dtString);
				var visitDates = ParseVisitDates(start_dtString, end_dtString);

				await _srmaModelService.SetVisitDates(srmaId, visitDates.startDate, visitDates.endDate);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateOfVisitPageModel::OnPostAsync::Exception - {Message}", ex.Message);
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
	
		private void ValidateDatesOfVisit(string startDateString, string endDateString)
		{

			if (!DateTimeHelper.TryParseExact(startDateString, out DateTime start_dt))
				throw new InvalidOperationException($"Start date {startDateString} is an invalid date");

			if (!string.IsNullOrEmpty(endDateString))
			{
				if (!DateTimeHelper.TryParseExact(endDateString, out DateTime end_dt))
					throw new InvalidOperationException($"End date {endDateString} is an invalid date");

				if (end_dt < start_dt)
					throw new InvalidOperationException($"Please ensure end date is same as or after start date.");
			}

		}
	
		private (DateTime startDate, DateTime? endDate) ParseVisitDates(string startDateString, string endDateString)
		{
			DateTime start_date;
			DateTime end_date;
			DateTime? end_dt = null;

			DateTimeHelper.TryParseExact(startDateString, out start_date);

			if (!string.IsNullOrEmpty(endDateString))
			{
				DateTimeHelper.TryParseExact(endDateString, out end_date);
				end_dt = end_date;
			}

			return (start_date, end_dt);
			
		}
	}
}