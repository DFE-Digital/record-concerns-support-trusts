﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
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

		[BindProperty(SupportsGet = true)]
		public int PageNumber { get; set; } = 1;
			
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
				var team = await _teamsService.GetCaseworkTeam(GetUserName());

				await RecordUserSignIn(team);

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

		/// <summary>
		/// This is a bit of a hack until integration with Azure AD has been completed and we are connected to the Azure GraphAPI which will then
		/// give us users directly from Azure AD. For now we will create empty teams, and use these team owners as the list of users in the system.
		/// Azure AD won't let us intercept the token being created and returns us to this page, so we will store that we have recorded a user having
		/// signed in, in Redis for 9 hours (slightly more than a standard shift) to avoid unnecessary calls to the academies API.
		/// </summary>
		/// <exception cref="System.NotImplementedException"></exception>
		private async Task RecordUserSignIn(Models.Teams.ConcernsTeamCaseworkModel team)
		{
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

			if (team.TeamMembers.Length == 0)
			{
				// This is a hack, but storing back an empty team will give us a 'user' for later (until GraphApi).
				await _teamsService.UpdateCaseworkTeam(team);
			}

			// record in redis that we have recorded a user state
			await _userStateCache.StoreData(GetUserName(), new UserState(GetUserName()));
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}
