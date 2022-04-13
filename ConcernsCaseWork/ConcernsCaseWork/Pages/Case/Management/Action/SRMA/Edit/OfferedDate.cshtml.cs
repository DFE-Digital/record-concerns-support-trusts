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
			try
			{
				_logger.LogInformation("Case::EditOfferedDatePageModel::OnGetAsync");

				var validationResponse = ValidateInputsForGet();
				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
				}
				else
				{
					SRMA = await srmaService.GetSRMAById(validationResponse.srmaId);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditOfferedDatePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync(string url)
		{
			try
			{
				_logger.LogInformation("Case::EditOfferedDatePageModel::OnPostEditOfferedDate");

				var validationResponse = ValidateInputsForPost();

				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
					return Page();
				}

				await srmaService.SetOfferedDate(validationResponse.srmaId, validationResponse.dateOffered);
				return Redirect($"/case/{validationResponse.caseId}/management/action/srma/{validationResponse.srmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditOfferedDatePageModel::OnPostEditOfferedDate::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private (List<string> validationErrors, long srmaId, long caseId) ValidateInputsForGet()
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

			return (validationErrors, srmaId, caseUrn);
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
	}
}