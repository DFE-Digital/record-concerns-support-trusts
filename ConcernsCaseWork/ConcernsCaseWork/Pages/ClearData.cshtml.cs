using Ardalis.GuardClauses;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClearDataPageModel : PageModel
	{
		private readonly ITrustCachedService _trustCachedService;
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly IUserStateCachedService _userStateCachedService;
		
		public ClearDataPageModel(
			IUserStateCachedService userStateCachedService, 
			ITrustCachedService trustCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_userStateCachedService = Guard.Against.Null(userStateCachedService);
			_trustCachedService = Guard.Against.Null(trustCachedService);
			_logger = Guard.Against.Null(logger);
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("ClearDataPageModel::OnGetAsync");
			
			if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("home");
			
			await _userStateCachedService.ClearData(User.Identity?.Name);
			await _trustCachedService.ClearData();

			return RedirectToPage("home");
		}
	}
}