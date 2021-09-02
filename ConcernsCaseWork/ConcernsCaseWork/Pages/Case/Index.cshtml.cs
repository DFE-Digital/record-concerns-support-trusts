using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.TRAMS.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICasesCachedService _casesCachedService;
		private readonly ILogger<IndexModel> _logger;
		
		private const int SearchQueryMinLength = 3;
		
		public IndexModel(ITrustModelService trustModelService, ICasesCachedService casesCachedService, ILogger<IndexModel> logger)
		{
			_trustModelService = trustModelService;
			_casesCachedService = casesCachedService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogInformation("Case::IndexModel::OnGetTrustsPartial");
			
				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < SearchQueryMinLength)
				{
					return Partial("_TrustsSearchResult", Array.Empty<TrustSummaryModel>());
				}

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);
				
				return Partial("_TrustsSearchResult", trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::IndexModel::OnGetTrustsSearchResult::Exception - {ex.Message}");
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public ActionResult OnGetSelectedTrust(string selectedTrust)
		{
			try
			{
				_logger.LogInformation("Case::IndexModel::OnGetSelectedTrust");
				
				// Double check selected trust.
				if (string.IsNullOrEmpty(selectedTrust) || selectedTrust.Contains("-") || selectedTrust.Length < SearchQueryMinLength)
				{
					throw new Exception($"Case::IndexModel::OnGetSelectedTrust::Selected trust is incorrect - {selectedTrust}");
				}

				// Store CaseState into cache.
				_casesCachedService.CreateCaseData(User.Identity.Name, new CaseStateData { TrustUkPrn = selectedTrust });

				return new JsonResult(new { redirectUrl = Url.Page("Details") });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::IndexModel::OnGetSelectedTrust::Exception - {ex.Message}");
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}