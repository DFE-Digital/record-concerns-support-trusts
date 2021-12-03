using ConcernsCaseWork.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Admin
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class IndexPageModel : PageModel
    {
	    private readonly IRbacManager _rbacManager;
	    private readonly ILogger<IndexPageModel> _logger;

	    public IDictionary<string, RoleClaimWrapper> UsersRole { get; private set; }
	    
	    public IndexPageModel(IRbacManager rbacManager, ILogger<IndexPageModel> logger)
	    {
		    _rbacManager = rbacManager;
		    _logger = logger;
	    }
	    
        public async Task OnGetAsync()
        {
	        _logger.LogInformation("IndexPageModel::OnGetAsync");

	        // Check if logged user has admin role
	        var userRoles = await _rbacManager.GetUserRoles(User.Identity.Name);

	        if (userRoles.Contains(RoleEnum.Admin))
	        {
		        // Get all users, roles and claims
		        UsersRole = await _rbacManager.GetUsersRoles();
	        }
	        else
	        {
		        TempData["Error.Message"] = $"The logged user {User.Identity.Name} doesn't have the necessary Roles to view the page.";
	        }
        }
    }
}
