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
using Service.Redis.Teams;
using Ardalis.GuardClauses;

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
		private readonly ITeamsCachedService _teamsCachedService;
		private readonly ILogger<ClearDataPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public ClearDataPageModel(ICachedService cachedService, 
			ITypeCachedService typeCachedService, 
			IStatusCachedService statusCachedService, 
			IRatingCachedService ratingCachedService,
			ITrustCachedService trustCachedService,
			INtiUnderConsiderationStatusesCachedService ntiUnderConsiderationStatusesCachedService,
			INtiUnderConsiderationReasonsCachedService ntiUnderConsiderationReasonsCachedService,
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonCachedService,
			INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			ITeamsCachedService teamsCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_statusCachedService = Guard.Against.Null(statusCachedService);
			_ratingCachedService = Guard.Against.Null(ratingCachedService);
			_trustCachedService = Guard.Against.Null(trustCachedService);
			_typeCachedService = Guard.Against.Null(typeCachedService);
			_ntiUnderConsiderationStatusesCachedService = Guard.Against.Null(ntiUnderConsiderationStatusesCachedService);
			_ntiUnderConsiderationReasonsCachedService = Guard.Against.Null(ntiUnderConsiderationReasonsCachedService);
			_ntiWarningLetterReasonCachedService = Guard.Against.Null(ntiWarningLetterReasonCachedService);
			_ntiWarningLetterStatusesCachedService = Guard.Against.Null(ntiWarningLetterStatusesCachedService);
			_teamsCachedService = Guard.Against.Null(teamsCachedService);
			_cachedService = Guard.Against.Null(cachedService);
			_logger = Guard.Against.Null(logger);
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
			await _ntiUnderConsiderationStatusesCachedService.ClearData();
			await _ntiUnderConsiderationReasonsCachedService.ClearData();
			await _ntiWarningLetterReasonCachedService.ClearData();
			await _ntiWarningLetterStatusesCachedService.ClearData();
			await _teamsCachedService.ClearData(User.Identity.Name);

			return RedirectToPage("home");
		}
	}
}