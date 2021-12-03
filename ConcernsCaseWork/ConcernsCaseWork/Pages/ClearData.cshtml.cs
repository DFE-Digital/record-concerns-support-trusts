using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Ratings;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClearDataPageModel : PageModel
	{
		private readonly IStatusCachedService _statusCachedService;
		private readonly IRatingCachedService _ratingCachedService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public ClearDataPageModel(ICachedService cachedService, 
			ITypeCachedService typeCachedService, 
			IStatusCachedService statusCachedService, 
			IRatingCachedService ratingCachedService,
			ITrustCachedService trustCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_trustCachedService = trustCachedService;
			_typeCachedService = typeCachedService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("ClearDataPageModel::OnGetAsync");
			
			if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("home");
			
			await _cachedService.ClearData(User.Identity.Name);
			await _typeCachedService.ClearData();
			await _statusCachedService.ClearData();
			await _ratingCachedService.ClearData();
			await _trustCachedService.ClearData();

			return RedirectToPage("home");
		}
	}
}