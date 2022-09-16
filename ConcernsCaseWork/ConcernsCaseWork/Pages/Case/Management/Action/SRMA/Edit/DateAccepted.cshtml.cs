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
	public class EditDateAcceptedPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateAcceptedPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }

		public EditDateAcceptedPageModel(ISRMAService srmaModelService, ILogger<EditDateAcceptedPageModel> logger)
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
				_logger.LogInformation("Case::Action::SRMA::EditDateAcceptedPageModel::OnGetAsync");

				(caseUrn, srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateAcceptedPageModel::OnGetAsync::Exception - {Message}", ex.Message);
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
				_logger.LogInformation("Case::Action::SRMA::EditDateAcceptedPageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				var dtr_day = Request.Form["dtr-day"].ToString();
				var dtr_month = Request.Form["dtr-month"].ToString();
				var dtr_year = Request.Form["dtr-year"].ToString();
				DateTime? acceptedDate = null;

				var dtString = string.IsNullOrEmpty(dtr_day) && string.IsNullOrEmpty(dtr_month) && string.IsNullOrEmpty(dtr_year) ?
				String.Empty : $"{dtr_day}-{dtr_month}-{dtr_year}";

				if (!string.IsNullOrEmpty(dtString))
				{
					if (!DateTimeHelper.TryParseExact(dtString, out DateTime dt))
					{
						throw new InvalidOperationException($"{dtString} is an invalid date");
					}

					acceptedDate = dt;
				}

				await _srmaModelService.SetDateAccepted(srmaId, acceptedDate);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateAcceptedPageModel::OnPostAsync::Exception - {Message}", ex.Message);
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