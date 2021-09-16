using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Trusts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICachedService _cachedService;
		private readonly ILogger<IndexPageModel> _logger;
		
		private const int SearchQueryMinLength = 3;
		
		public IndexPageModel(ITrustModelService trustModelService, ICachedService cachedService, ILogger<IndexPageModel> logger)
		{
			_trustModelService = trustModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogInformation("Case::IndexPageModel::OnGetTrustsPartial");
			
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
				_logger.LogError($"Case::IndexPageModel::OnGetTrustsSearchResult::Exception - {ex.Message}");
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public async Task<ActionResult> OnGetSelectedTrust(string selectedTrust)
		{
			try
			{
				_logger.LogInformation("Case::IndexPageModel::OnGetSelectedTrust");
				
				// Double check selected trust.
				if (string.IsNullOrEmpty(selectedTrust) || selectedTrust.Contains("-") || selectedTrust.Length < SearchQueryMinLength)
				{
					throw new Exception($"Case::IndexModel::OnGetSelectedTrust::Selected trust is incorrect - {selectedTrust}");
				}

				// Store CaseState into cache.
				var userState = await _cachedService.GetData<UserState>(User.Identity.Name) ?? new UserState();
				userState.TrustUkPrn = selectedTrust;
				await _cachedService.StoreData(User.Identity.Name, userState);

				return new JsonResult(new { redirectUrl = Url.Page("Details") });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::IndexPageModel::OnGetSelectedTrust::Exception - {ex.Message}");
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}