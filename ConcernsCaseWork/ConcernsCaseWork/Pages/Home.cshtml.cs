using ConcernsCaseWork.Models;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
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
        public bool UserAsRoleLeader { get; private set; }
        
        public HomePageModel(ICaseModelService caseModelService, IRbacManager rbacManager, ILogger<HomePageModel> logger)
        {
            _caseModelService = caseModelService;
            _rbacManager = rbacManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
	        _logger.LogInformation("HomePageModel::OnGetAsync executed");

	        // Check if logged user as role leader
	        // And get all live cases for each caseworker
	        Task<IList<HomeModel>> liveCasesTeamLead = null;
	        var userRoleClaimWrapper = await _rbacManager.GetUserRoleClaimWrapper(User.Identity.Name);
	        if (userRoleClaimWrapper.Roles.Contains(RoleEnum.Leader))
	        {
		        var groupUsers = userRoleClaimWrapper.Users;
		        UserAsRoleLeader = true;
		        
		        liveCasesTeamLead = _caseModelService.GetCasesByCaseworkerAndStatus(groupUsers, StatusEnum.Live);
	        }
	        
	        // Get all live and monitoring cases
	        Task<IList<HomeModel>> liveCases = _caseModelService.GetCasesByCaseworkerAndStatus(User.Identity.Name, StatusEnum.Live);

	        // Wait until all tasks are completed
	        if (liveCasesTeamLead != null)
	        {
		        await Task.WhenAll(liveCases, liveCasesTeamLead);

		        // Assign responses to UI public properties
		        CasesActive = liveCases.Result;
		        CasesTeamActive = liveCasesTeamLead.Result;
	        }
	        else
	        {
		        Task.WaitAll(liveCases);
		        
		        // Assign responses to UI public properties
		        CasesActive = liveCases.Result;
	        }
        }
    }
}
