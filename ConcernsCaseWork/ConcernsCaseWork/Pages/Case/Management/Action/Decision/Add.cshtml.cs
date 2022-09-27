
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Service.Redis.Nti;
using ConcernsCaseWork.Services.Nti;
using Service.TRAMS.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;

		public AddPageModel(ILogger<AddPageModel> logger)
		{
			_logger = logger;
		}

		public long CaseUrn { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				SetPagePropertiesFromRouteValues();

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private long SetPagePropertiesFromRouteValues()
		{
			return CaseUrn = GetRouteCaseUrn();
		}


		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
		
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private CaseUrn GetRouteCaseUrn()
		{
			_logger.LogMethodEntered();
			var caseUrnValueObj = RouteData.Values["urn"];
			if (caseUrnValueObj == null || !long.TryParse(caseUrnValueObj.ToString(), out long caseUrnValue))
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			// TODO: Explain this to Elijah
			return (CaseUrn)caseUrnValue;
		}
	}
}