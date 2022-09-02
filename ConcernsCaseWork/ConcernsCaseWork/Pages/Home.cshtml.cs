﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using Service.Redis.Users;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class HomePageModel : PageModel
	{
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly IUserStateCachedService _userStateCache;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<HomePageModel> _logger;
		private readonly ITeamsModelService _teamsService;
		private readonly IRbacManager _rbacManager;

		public IList<HomeModel> CasesActive { get; private set; }
		public IList<HomeModel> CasesTeamActive { get; private set; }

		public HomePageModel(ICaseModelService caseModelService,
			IRbacManager rbacManager,
			ILogger<HomePageModel> logger,
			ITeamsModelService teamsService,
			IUserStateCachedService userStateCache,
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_rbacManager = Guard.Against.Null(rbacManager);
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("HomePageModel::OnGetAsync executed");

			// Display all live cases for the current user.
			// And in addition display live cases belonging to other users that the current user has expressed an interest in seeing.

			// Check if logged user as role leader
			// And get all live cases for each caseworker

			// cases belonging to this user
			Task<IList<HomeModel>> currentUserLiveCases = _caseModelService.GetCasesByCaseworkerAndStatus(GetUserName(), StatusEnum.Live);

			// get any team members defined
			var team = await _teamsService.GetCaseworkTeam(GetUserName());

			var liveCasesTeamLeadTask = _caseModelService.GetCasesByCaseworkerAndStatus(team.TeamMembers, StatusEnum.Live);

			var recordUserSignedTask = RecordUserSignIn(team);

			await Task.WhenAll(currentUserLiveCases, liveCasesTeamLeadTask, recordUserSignedTask);

			// Assign responses to UI public properties
			CasesActive = currentUserLiveCases.Result;
			CasesTeamActive = liveCasesTeamLeadTask.Result;
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

			if (userState is not null)
			{
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
