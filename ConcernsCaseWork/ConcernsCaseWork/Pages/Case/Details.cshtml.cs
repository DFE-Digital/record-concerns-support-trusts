using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsPageModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		private readonly ICachedService _cachedService;

		private const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		private const string ErrorOnPostPage = "An error occurred posting the form, please try again. If the error persists contact the service administrator.";
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public DetailsPageModel(ITrustModelService trustModelService, ICachedService cachedService, ILogger<DetailsPageModel> logger)
		{
			_trustModelService = trustModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnGetAsync");
				
				// Get cached data from case page.
				var caseStateModel = await _cachedService.GetData<CaseState>(User.Identity.Name);
				if (caseStateModel is null)
				{
					throw new Exception("Cache CaseStateData is null");
				}

				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseStateModel.TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPost()
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnPost");

				var type = Request.Form["type"];
				var subType = Request.Form["subType"];
				var ragRating = Request.Form["riskRating"];
				var issueDetail = Request.Form["issue-detail"];
				var currentStatusDetail = Request.Form["current-status-detail"];
				var nextStepsDetail = Request.Form["next-steps-detail"];
				var resolutionStrategyDetail = Request.Form["resolution-strategy-detail"];

				if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(subType) && 
				    !string.IsNullOrEmpty(ragRating) && !string.IsNullOrEmpty(issueDetail))
				{
					// Generate UUID
					var uuid = Guid.NewGuid();

					// Store in cache for now
					var caseStateModel = await _cachedService.GetData<CaseState>(User.Identity.Name);
					
					
					
					
					return RedirectToPage("Management", new { id = uuid });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsModel::OnPost::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Page();
		}
	}
}