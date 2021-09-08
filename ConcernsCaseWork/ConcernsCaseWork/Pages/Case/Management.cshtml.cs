using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ManagementPageModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICachedService _cachedService;
		private readonly ILogger<DetailsPageModel> _logger;
		
		public ManagementPageModel(ITrustModelService trustModelService, ICachedService cachedService, ILogger<DetailsPageModel> logger)
		{
			_trustModelService = trustModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public void OnGet(string id)
		{
			_logger.LogInformation($"ID - {id}");
		}
	}
}