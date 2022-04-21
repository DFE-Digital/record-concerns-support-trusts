using ConcernsCaseWork.Enums;
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

namespace ConcernsCaseWork.Pages.Case.Management.Action.Srma
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ResolvePageModel : AbstractPageModel
	{
		private readonly ILogger<ResolvePageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		public ResolvePageModel(
			ISRMAService srmaModelService, ILogger<ResolvePageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::ResolvePageModel::OnGetAsync");

			try
			{
				GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::ResolvePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var caseUrn = GetRouteData();

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::ResolvePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
		}

		private (long caseUrn, long srmaId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			return (caseUrn, srmaId);
		}
	}
}