using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Ratings;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
using Service.Redis.NtiWarningLetter;
using System.Threading.Tasks;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.Users;

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
		private readonly INtiUnderConsiderationStatusesCachedService _ntiUnderConsiderationStatusesCachedService;
		private readonly INtiUnderConsiderationReasonsCachedService _ntiUnderConsiderationReasonsCachedService;
		private readonly INtiWarningLetterReasonsCachedService _ntiWarningLetterReasonCachedService; 
		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService; 
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly IUserStateCachedService _userStateCachedService;
		
		public ClearDataPageModel(
			IUserStateCachedService userStateCachedService, 
			ITypeCachedService typeCachedService, 
			IStatusCachedService statusCachedService, 
			IRatingCachedService ratingCachedService,
			ITrustCachedService trustCachedService,
			INtiUnderConsiderationStatusesCachedService ntiUnderConsiderationStatusesCachedService,
			INtiUnderConsiderationReasonsCachedService ntiUnderConsiderationReasonsCachedService,
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonCachedService,
			INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_trustCachedService = trustCachedService;
			_typeCachedService = typeCachedService;
			_ntiUnderConsiderationStatusesCachedService = ntiUnderConsiderationStatusesCachedService;
			_ntiUnderConsiderationReasonsCachedService = ntiUnderConsiderationReasonsCachedService;
			_ntiWarningLetterReasonCachedService = ntiWarningLetterReasonCachedService;
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_userStateCachedService = userStateCachedService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("ClearDataPageModel::OnGetAsync");
			
			if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("home");
			
			await _userStateCachedService.ClearData(User.Identity?.Name);
			await _typeCachedService.ClearData();
			await _statusCachedService.ClearData();
			await _ratingCachedService.ClearData();
			await _trustCachedService.ClearData();
			await _ntiUnderConsiderationStatusesCachedService.ClearData();
			await _ntiUnderConsiderationReasonsCachedService.ClearData();
			await _ntiWarningLetterReasonCachedService.ClearData();
			await _ntiWarningLetterStatusesCachedService.ClearData();

			return RedirectToPage("home");
		}
	}
}