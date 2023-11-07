using Ardalis.GuardClauses;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Types;
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
		private readonly ITypeCachedService _typeCachedService;
		private readonly ITeamsCachedService _teamsCachedService;
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly IUserStateCachedService _userStateCachedService;
		
		public ClearDataPageModel(
			IUserStateCachedService userStateCachedService, 
			ITypeCachedService typeCachedService, 
			ITrustCachedService trustCachedService,
			ITeamsCachedService teamsCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_userStateCachedService = Guard.Against.Null(userStateCachedService);
			_trustCachedService = Guard.Against.Null(trustCachedService);
			_typeCachedService = Guard.Against.Null(typeCachedService);
			_teamsCachedService = Guard.Against.Null(teamsCachedService);
			_logger = Guard.Against.Null(logger);
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("ClearDataPageModel::OnGetAsync");
			
			if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("home");
			
			await _userStateCachedService.ClearData(User.Identity?.Name);
			await _typeCachedService.ClearData();
			await _trustCachedService.ClearData();
			await _teamsCachedService.ClearData(User.Identity?.Name);

			return RedirectToPage("home");
		}
	}
}