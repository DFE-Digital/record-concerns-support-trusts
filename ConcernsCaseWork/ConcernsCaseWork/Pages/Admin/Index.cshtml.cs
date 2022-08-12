using ConcernsCaseWork.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Admin
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class IndexPageModel : PageModel
    {
	    private readonly IUserRoleCachedService _userRoleCachedService;
	    private readonly IRbacManager _rbacManager;
	    private readonly ILogger<IndexPageModel> _logger;
	    
	    public IDictionary<string, RoleClaimWrapper> UsersRole { get; private set; }
	    // Disable editing on the page when it's admin username, re-think further inline when AD integration.
	    public const string AdminUserName = UserRoleMap.AdminUserName;
	    
	    public IndexPageModel(IRbacManager rbacManager, IUserRoleCachedService userRoleCachedService, ILogger<IndexPageModel> logger)
	    {
		    _userRoleCachedService = userRoleCachedService;
		    _rbacManager = rbacManager;
		    _logger = logger;
	    }
	    
        public async Task OnGetAsync()
        {
	        _logger.LogInformation("Admin::IndexPageModel::OnGetAsync");

	        await LoadPage();
        }

        public async Task OnGetClearCache()
        {
	        _logger.LogInformation("Admin::IndexPageModel::OnGetClearCache");
	        
	        await _userRoleCachedService.ClearData();
			await LoadPage();

			Redirect("/admin");
        }
        
        private async Task LoadPage()
        {
			// Check if logged user has admin role
	        var userRoleClaimWrapper = await _rbacManager.GetUserRoleClaimWrapper(User.Identity.Name);

	        if (userRoleClaimWrapper.Roles.Contains(RoleEnum.Admin))
	        {
		        // Get all users, roles and claims
		        UsersRole = await _rbacManager.GetUsersRoles();
	        }
	        else
	        {
		        TempData["Error.Message"] = $"The logged user {User.Identity.Name} doesn't have the necessary Roles to view the page.";
	        }

	        Page();
        }
    }
}
