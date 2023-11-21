using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Teams;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class HomePageModel : AbstractPageModel
	{
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly IUserStateCachedService _userStateCache;
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly ILogger<HomePageModel> _logger;
		private readonly ITeamsModelService _teamsService;
		private TelemetryClient _telemetry;
		public List<ActiveCaseSummaryModel> ActiveCases { get; private set; }

		public PaginationModel Pagination { get; set; }
			
		public HomePageModel(
			ILogger<HomePageModel> logger,
			ITeamsModelService teamsService,
			ICaseSummaryService caseSummaryService,
			IUserStateCachedService userStateCache,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient)
		{
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
			_telemetry = Guard.Against.Null(telemetryClient);
		}

		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogInformation("HomePageModel::OnGetAsync executed");

			try
			{
				await RecordUserSignIn();

				var activeCaseGroup = await _caseSummaryService.GetActiveCaseSummariesByCaseworker(GetUserName(), PageNumber);
				ActiveCases = activeCaseGroup.Cases;

				Pagination = activeCaseGroup.Pagination;
			}
			catch (Exception ex)
			{
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		private async Task RecordUserSignIn()
		{
			var member = await _teamsService.CheckMemberExists(GetUserName());

			if (member == null)
			{
				// Add the user if they don't already exist
				var model = new ConcernsTeamCaseworkModel(GetUserName(), Array.Empty<string>());
				await _teamsService.UpdateCaseworkTeam(model);
			}

			var userState = await _userStateCache.GetData(GetUserName());

			if (userState is not null && !String.IsNullOrWhiteSpace(userState.UserName))
			{
				AppInsightsHelper.LogEvent(_telemetry, new AppInsightsModel()
				{
					EventName = "HOME",
					EventDescription = "User logging in and accessing home page.",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});
				return;
			}

			// record in redis that we have recorded a user state
			await _userStateCache.StoreData(GetUserName(), new UserState(GetUserName()));
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}
