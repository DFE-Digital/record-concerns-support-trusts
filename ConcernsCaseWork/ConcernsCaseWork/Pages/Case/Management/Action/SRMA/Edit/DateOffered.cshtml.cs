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
	public class EditDateOfferedPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateOfferedPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }

		public EditDateOfferedPageModel(ISRMAService srmaService, ILogger<EditDateOfferedPageModel> logger)
		{
			_srmaModelService = srmaService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::EditDateOfferedPageModel::OnGetAsync");

				(long caseUrn, long srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);
				
				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}/closed");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditDateOfferedPageModel::OnGetAsync::Exception - {Message}", ex.Message);
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
				_logger.LogInformation("Case::Action::SRMA::EditDateOfferedPageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();

				var dtr_day = Request.Form["dtr-day"].ToString();
				var dtr_month = Request.Form["dtr-month"].ToString();
				var dtr_year = Request.Form["dtr-year"].ToString();

				var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";

				if (!DateTimeHelper.TryParseExact(dtString, out DateTime dt))
				{
					throw new InvalidOperationException($"{dtString} is an invalid date");
				}

				await _srmaModelService.SetOfferedDate(srmaId, dt);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditDateOfferedPageModel::OnPostAsync::Exception - {Message}", ex.Message);
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