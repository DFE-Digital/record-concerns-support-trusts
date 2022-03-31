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
		private readonly ISRMAService SRMAService;

		public int NotesMaxLength => 500;
		public IEnumerable<RadioItem> SRMAStatuses => getStatuses();

		public AddPageModel(
			ISRMAService SRMAService, ILogger<AddPageModel> logger)
		{
			this.SRMAService = SRMAService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::AddPageModel::OnGetAsync");
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var validationResp = ValidateAndCreateSRMA();
			if (validationResp.validationErrors?.Count > 0)
			{
				TempData["SRMA.Message"] = validationResp.validationErrors;
				return Page();
			}

			await SRMAService.SaveSRMA(validationResp.newSRMA);

			return Redirect($"/case/{validationResp.newSRMA.CaseUrn}/management");
		}

		private (CaseActionModels.SRMAModel newSRMA, List<string> validationErrors) ValidateAndCreateSRMA()
		{
			var srma = new CaseActionModels.SRMAModel();
			var validationErrors = new List<string>();

			if (!long.TryParse(Convert.ToString(RouteData.Values["urn"]), out long caseUrn))
			{
				validationErrors.Add("Invalid case Id");
			}
			else
			{
				srma.CaseUrn = caseUrn;
			}

			var status = Request.Form["status"];

			if (string.IsNullOrEmpty(status))
			{
				validationErrors.Add("SRMA status not selected");
			}

			if (!Enum.TryParse<SRMAStatus>(status, ignoreCase: true, out SRMAStatus srmaStatus))
			{
				_logger.Log(LogLevel.Error, $"Can't parse SRMA status ");
			}
			else
			{
				srma.Status = srmaStatus;
			}

			var dtString = $"{Request.Form["dtr-day"]}-{Request.Form["dtr-month"]}-{Request.Form["dtr-year"]}";
			if (!DateTime.TryParseExact(dtString, "dd-MM-yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out DateTime dateOffered))
			{
				validationErrors.Add("SRMA offered date is not valid");
			}
			else
			{
				srma.DateOffered = dateOffered;
			}

			if (!string.IsNullOrEmpty(Request.Form["srma-notes"]))
			{
				var notes = Request.Form["srma-notes"].ToString();
				if (notes.Length > NotesMaxLength)
				{
					validationErrors.Add($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}

			return (srma, validationErrors);
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
	}
}