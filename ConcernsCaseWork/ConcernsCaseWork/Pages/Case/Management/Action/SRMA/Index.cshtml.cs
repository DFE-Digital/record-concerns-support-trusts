﻿using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using CaseActionModels = ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Srma
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
			if (long.TryParse(Convert.ToString(RouteData.Values["urn"]), out long caseUrn) == false || caseUrn <= 0)
			{
				TempData["Error.Message"] = "Case Id Not Provided";
				return;
			}

			if (long.TryParse(Convert.ToString(RouteData.Values["srmaId"]), out long srmaId) == false || srmaId <= 0)
			{
				TempData["Error.Message"] = "SRMA Id Not Provided";
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