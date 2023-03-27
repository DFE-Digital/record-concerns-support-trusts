using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosedPageModel : AbstractPageModel
	{
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ILogger<ClosedPageModel> _logger;
		private TelemetryClient _telemetry;
		
		public List<ClosedCaseSummaryModel> ClosedCases { get; private set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);
		
		public ClosedPageModel(ICaseSummaryService caseSummaryService, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			ILogger<ClosedPageModel> logger,
			TelemetryClient telemetryClient)
		{
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
			_claimsPrincipalHelper =Guard.Against.Null( claimsPrincipalHelper);
			_logger =Guard.Against.Null( logger);
			_telemetry = Guard.Against.Null(telemetryClient);
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("ClosedPageModel::OnGetAsync executed");
				AppInsightsHelper.LogEvent(_telemetry, new AppInsightsModel()
				{
					EventName = "VIEW CLOSED",
					EventDescription = "Accessing and viewing closed cases.",
					EventPayloadJson = "",
					EventUserName = GetUserName()
				});
				ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByCaseworker(GetUserName());
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosedPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				ClosedCases = new List<ClosedCaseSummaryModel>();
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}