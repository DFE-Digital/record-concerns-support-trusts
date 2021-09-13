using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class ClearDataPageModel : PageModel
	{
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public ClearDataPageModel(ICachedService cachedService, ILogger<ClearDataPageModel> logger)
		{
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("ClearDataPageModel::OnGetAsync");
			
			if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("home");
			
			await _cachedService.ClearData(User.Identity.Name);

			return RedirectToPage("home");
		}
	}
}