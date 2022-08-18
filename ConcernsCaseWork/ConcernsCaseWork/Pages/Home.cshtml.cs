using ConcernsCaseWork.Models;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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
		private readonly IRbacManager _rbacManager;

		public IList<HomeModel> CasesActive { get; private set; }
		public IList<HomeModel> CasesTeamActive { get; private set; }

		public HomePageModel(ICaseModelService caseModelService, IRbacManager rbacManager, ILogger<HomePageModel> logger)
		{
			_caseModelService = caseModelService;
			_rbacManager = rbacManager;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("HomePageModel::OnGetAsync executed");

			// Display all live cases for the current user.
			// And in addition display live cases belonging to other users that the current user has expressed an interest in seeing.

			// Check if logged user as role leader
			// And get all live cases for each caseworker
			Task<IList<HomeModel>> liveCasesTeamLead = null;

			// cases belonging to this user
			Task<IList<HomeModel>> currentUserLiveCases = _caseModelService.GetCasesByCaseworkerAndStatus(User.Identity.Name, StatusEnum.Live);

			// other uses to show
			var userRoleClaimWrapper = await _rbacManager.GetUserRoleClaimWrapper(User.Identity.Name);
			var groupUsers = userRoleClaimWrapper.Users;
			liveCasesTeamLead = _caseModelService.GetCasesByCaseworkerAndStatus(groupUsers, StatusEnum.Live);
			{
				await Task.WhenAll(currentUserLiveCases, liveCasesTeamLead);

				// Assign responses to UI public properties
				CasesActive = currentUserLiveCases.Result;
				CasesTeamActive = liveCasesTeamLead.Result;
			}
		}
	}
}
