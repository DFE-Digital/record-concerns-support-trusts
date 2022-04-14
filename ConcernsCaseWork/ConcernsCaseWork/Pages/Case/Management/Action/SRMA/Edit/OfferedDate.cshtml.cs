using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditOfferedDatePageModel : AbstractPageModel
	{
		private readonly ISRMAService srmaService;
		private readonly ILogger<EditOfferedDatePageModel> _logger;

		public SRMAModel SRMA { get; set; }

		public EditOfferedDatePageModel(ISRMAService srmaService, ILogger<EditOfferedDatePageModel> logger)
		{
			this.srmaService = srmaService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			long srmaId = 0;

			try
			{
				_logger.LogInformation("Case::EditOfferedDatePageModel::OnGetAsync");

				(_, srmaId) = GetRouteData();

				SRMA = await srmaService.GetSRMAById(srmaId);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditOfferedDatePageModel::OnGetAsync::Exception - {Message}", ex.Message);
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
				_logger.LogInformation("Case::Action::SRMA::EditOfferedDatePageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();

				var dtr_day = Request.Form["dtr-day"].ToString();
				var dtr_month = Request.Form["dtr-month"].ToString();
				var dtr_year = Request.Form["dtr-year"].ToString();

				var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";

				if (!DateTimeHelper.TryParseExact(dtString, out DateTime dt))
				{
					throw new InvalidOperationException($"{dtString} is an invalid date");
				}

				await srmaService.SetOfferedDate(srmaId, dt);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditOfferedDatePageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private (List<string> validationErrors, long srmaId, DateTime dateOffered, long caseId) ValidateInputsForPost()
		{
			var validationErrors = new List<string>();
			
			var caseUrnStr = Convert.ToString(RouteData.Values["caseUrn"]);
			if (!long.TryParse(caseUrnStr, out long caseUrn))
			{
				validationErrors.Add("Invalid case Id");
			}

			var srmaIdStr = Convert.ToString(RouteData.Values["srmaId"]);
			if (!long.TryParse(srmaIdStr, out long srmaId))
			{
				validationErrors.Add("SRMA Id not found");
			}

			var dtString = $"{Request.Form["dtr-day"]}-{Request.Form["dtr-month"]}-{Request.Form["dtr-year"]}";

			if (!DateTimeHelper.TryParseExact(dtString, out DateTime dateOffered))
			{
				validationErrors.Add("SRMA offered date is not valid");
			}

			return (validationErrors, srmaId, dateOffered, caseUrn);
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