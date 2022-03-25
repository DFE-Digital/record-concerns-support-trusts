using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using CaseActionModels = ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaService;
		private readonly ILogger<IndexPageModel> _logger;

		public long CaseUrn { get; set; }
		public CaseActionModels.SRMA SRMA { get; set; }

		public IndexPageModel(ISRMAService srmaService, ILogger<IndexPageModel> logger)
		{
			_srmaService = srmaService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			var caseUrnValue = Convert.ToString(RouteData.Values["urn"]);
			if (caseUrnValue == null || !long.TryParse(caseUrnValue, out long caseUrn) || caseUrn <= 0)
			{
				TempData["Error.Message"] = "Case Id not provided or invalid";
				return;
			}

			var srmaIdValue = Convert.ToString(RouteData.Values["srmaId"]);
			if (srmaIdValue == null || !long.TryParse(srmaIdValue, out long srmaId) || srmaId <= 0)
			{
				TempData["Error.Message"] = "SRMA Id not provided or invalid";
				return;
			}

			CaseUrn = caseUrn;

			SRMA = await _srmaService.GetSRMAById(srmaId);
			if(SRMA == null)
			{
				TempData["SRMA.Message"] =  "Could not load this SRMA";
				return;
			}

		}

		public async Task<IActionResult> OnPostAsync()
		{

			return RedirectToPage("index");
		}
	}
}