using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Redis.Users;

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
			ITeamsCachedService teamsCachedService,
			ILogger<ClearDataPageModel> logger)
		{
			_userStateCachedService = Guard.Against.Null(userStateCachedService);
			_statusCachedService = Guard.Against.Null(statusCachedService);
			_ratingCachedService = Guard.Against.Null(ratingCachedService);
			_trustCachedService = Guard.Against.Null(trustCachedService);
			_typeCachedService = Guard.Against.Null(typeCachedService);
			_ntiUnderConsiderationStatusesCachedService = Guard.Against.Null(ntiUnderConsiderationStatusesCachedService);
			_ntiUnderConsiderationReasonsCachedService = Guard.Against.Null(ntiUnderConsiderationReasonsCachedService);
			_ntiWarningLetterReasonCachedService = Guard.Against.Null(ntiWarningLetterReasonCachedService);
			_ntiWarningLetterStatusesCachedService = Guard.Against.Null(ntiWarningLetterStatusesCachedService);
			_teamsCachedService = Guard.Against.Null(teamsCachedService);
			_logger = Guard.Against.Null(logger);
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
			await _teamsCachedService.ClearData(User.Identity?.Name);

			return RedirectToPage("home");
		}
	}
}