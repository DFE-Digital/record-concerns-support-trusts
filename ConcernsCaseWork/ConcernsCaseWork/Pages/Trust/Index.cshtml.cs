using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Users;
using Service.TRAMS.Trusts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Trust
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private readonly ILogger<IndexPageModel> _logger;
		
		private const int SearchQueryMinLength = 3;
		
		public IndexPageModel(ITrustModelService trustModelService, IUserStateCachedService userStateCache, ILogger<IndexPageModel> logger)
		{
			_trustModelService = trustModelService;
			_userStateCache = userStateCache;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogInformation("Trust::IndexPageModel::OnGetTrustsPartial");
				
				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < SearchQueryMinLength)
				{
					return new JsonResult(Array.Empty<TrustSearchModel>());
				}

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);

				return new JsonResult(trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnGetTrustsSearchResult::Exception - {Message}", ex.Message);
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public async Task<ActionResult> OnGetSelectedTrust(string trustUkPrn)
		{
			try
			{
				_logger.LogInformation("Trust::IndexPageModel::OnGetSelectedTrust");
				
				// Double check selected trust.
				if (string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains("-") || trustUkPrn.Length < SearchQueryMinLength)
					throw new Exception($"Trust::IndexModel::OnGetSelectedTrust::Selected trust is incorrect - {trustUkPrn}");
				
				// Store CaseState into cache.
				var userState = await _userStateCache.GetData(User.Identity?.Name) ?? new UserState();
				userState.TrustUkPrn = trustUkPrn;
				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(User.Identity?.Name, userState);

				return new JsonResult(new { redirectUrl = Url.Page("Overview", new { id = trustUkPrn }) });
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnGetSelectedTrust::Exception - {Message}", ex.Message);
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}