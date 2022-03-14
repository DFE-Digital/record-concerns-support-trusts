using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseActionModels = ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ISRMAService SRMAService;

		public int NotesMaxLength => 500;
		public IEnumerable<RadioItem> SRMAStatuses => getStatuses();

		private IEnumerable<RadioItem> getStatuses()
		{
			return new RadioItem[]
			{
				new RadioItem { Text = "Trust considering" },
				new RadioItem { Text = "Preparing for deployment" },
				new RadioItem { Text = "Deployed" }
			};
		}

		public AddPageModel(
			ILogger<AddPageModel> logger, ISRMAService SRMAService)
		{
			_logger = logger;
			this.SRMAService = SRMAService;
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::AddPageModel::OnGetAsync");
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var validationResp = ValidateAndCreateSRMA();
			if(validationResp.validationErrors?.Count > 0)
			{
				TempData["SRMA.Message"] = validationResp.validationErrors;
				return Page();
			}

			await SRMAService.SaveSRMA(validationResp.newSRMA);

			// dtr-day, dtr-month, dtr-year
			// srma-notes
			//case/ @homeModel.CaseUrn / management
			return RedirectToPage("case/managementllll", new { urn = validationResp.newSRMA.CaseUrn});
		}

		private (CaseActionModels.SRMA newSRMA, List<string> validationErrors) ValidateAndCreateSRMA()
		{
			bool validationFailed;
			var srma = new CaseActionModels.SRMA();
			var validationErrors = new List<string>();

			var status = Request.Form["status"];

			if (string.IsNullOrEmpty(status))
			{
				validationErrors.Add("SRMA status not selected");
				validationFailed = true;
			}

			if (!Enum.TryParse<SRMAStatus>(status, ignoreCase:true, out SRMAStatus srmaStatus))
			{
				_logger.Log(LogLevel.Error, $"Can't parse SRMA status ");
				validationFailed = true;
			}
			else
			{
				srma.Status = srmaStatus;
			}

			if (!DateTime.TryParse($"{Request.Form["dtr-day"]}-{Request.Form["dtr-month"]}-{Request.Form["dtr-year"]}", out DateTime dateOffered))
			{
				validationErrors.Add("SRMA offered date is not valid");
				validationFailed = true;
			}
			else
			{
				srma.DateOffered = dateOffered;
			}

			if (string.IsNullOrEmpty(Request.Form["srma-notes"]))
			{
				validationErrors.Add("Notes not provided");
			}
			else
			{
				srma.Notes = Request.Form["srma-notes"];
			}

			if(!long.TryParse(Convert.ToString(RouteData.Values["urn"]), out long caseUrn))
			{
				validationErrors.Add("Invalid case Id");
			}
			else
			{
				srma.CaseUrn = caseUrn;
			}

			return (srma, validationErrors);
		}
	}
}