using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICasesCachedService _casesCachedService;
		private readonly ILogger<DetailsModel> _logger;

		private const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		private const string ErrorOnPostPage = "An error occurred posting the form, please try again. If the error persists contact the service administrator.";
		private const string RedirectToPageCaseDetailsRecord = "Record";
		private const string RedirectToPageCaseDetailsSafeguarding = "Safeguarding";
		private const string RedirectToPageCaseDetailsConcern = "Concern";
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public DetailsModel(ITrustModelService trustModelService, ICasesCachedService casesCachedService, ILogger<DetailsModel> logger)
		{
			_trustModelService = trustModelService;
			_casesCachedService = casesCachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnGetAsync");
				
				// Get temp data from case page.
				var caseStateData = await _casesCachedService.GetCaseData<CaseStateData>(User.Identity.Name);
				if (caseStateData is null)
				{
					throw new Exception("Cache CaseStateData is null");
				}

				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseStateData.TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public IActionResult OnPost(string caseType)
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnPost");
				
				if (string.IsNullOrEmpty(caseType))
				{
					throw new Exception("Case type is null or empty");
				}

				return caseType switch
				{
					"record" => new RedirectToPageResult(RedirectToPageCaseDetailsRecord),
					"safeguarding" => new RedirectToPageResult(RedirectToPageCaseDetailsSafeguarding),
					"concern" => new RedirectToPageResult(RedirectToPageCaseDetailsConcern),
					_ => new RedirectToPageResult(RedirectToPageCaseDetailsRecord)
				};
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