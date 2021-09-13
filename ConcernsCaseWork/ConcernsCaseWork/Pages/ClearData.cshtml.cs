using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Rating;
using Service.Redis.Status;
using Service.Redis.Type;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class ClearDataPageModel : PageModel
	{
		private readonly IStatusCachedService _statusCachedService;
		private readonly IRatingCachedService _ratingCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public ClearDataPageModel(ICachedService cachedService, ITypeCachedService typeCachedService, 
			IStatusCachedService statusCachedService, IRatingCachedService ratingCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
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

			return RedirectToPage("home");
		}
	}
}