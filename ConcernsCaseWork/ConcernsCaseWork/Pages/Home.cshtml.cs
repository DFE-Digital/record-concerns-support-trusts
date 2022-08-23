using Ardalis.GuardClauses;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sentry;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class HomePageModel : PageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<HomePageModel> _logger;
		private readonly ITeamsService _teamsService;
		private readonly IRbacManager _rbacManager;

		public IList<HomeModel> CasesActive { get; private set; }
		public IList<HomeModel> CasesTeamActive { get; private set; }

		public HomePageModel(ICaseModelService caseModelService, IRbacManager rbacManager, ILogger<HomePageModel> logger, ITeamsService teamsService)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_rbacManager = Guard.Against.Null(rbacManager);
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("HomePageModel::OnGetAsync executed");

			// Display all live cases for the current user.
			// And in addition display live cases belonging to other users that the current user has expressed an interest in seeing.

			// Check if logged user as role leader
			// And get all live cases for each caseworker

			// cases belonging to this user
			Task<IList<HomeModel>> currentUserLiveCases = _caseModelService.GetCasesByCaseworkerAndStatus(User.Identity.Name, StatusEnum.Live);

			// get any team members defined
			var team = await _teamsService.GetCaseworkTeam(User.Identity.Name);

			var liveCasesTeamLead = _caseModelService.GetCasesByCaseworkerAndStatus(team.TeamMembers, StatusEnum.Live);

			await Task.WhenAll(currentUserLiveCases, liveCasesTeamLead);

			// Assign responses to UI public properties
			CasesActive = currentUserLiveCases.Result;
			CasesTeamActive = liveCasesTeamLead.Result;
		}
	}
}
