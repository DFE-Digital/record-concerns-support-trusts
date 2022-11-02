using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.Trusts;
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
		private readonly IUserStateCachedService _userStateCache;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		private const int SearchQueryMinLength = 3;
		
		public IndexPageModel(ITrustModelService trustModelService, IUserStateCachedService userStateCache, ILogger<IndexPageModel> logger, IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		}
		
		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogInformation("Case::IndexPageModel::OnGetTrustsSearchResult");
				
				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < SearchQueryMinLength)
					return new JsonResult(Array.Empty<TrustSearchModel>());

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);

				return new JsonResult(trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::IndexPageModel::OnGetTrustsSearchResult::Exception - {Message}", ex.Message);
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public async Task<ActionResult> OnGetSelectedTrust(string trustUkPrn)
		{
			try
			{
				_logger.LogInformation("Case::IndexPageModel::OnGetSelectedTrust");
				
				// Double check selected trust.
				if (string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains("-") || trustUkPrn.Length < SearchQueryMinLength)
					throw new Exception($"Selected trust is incorrect - {trustUkPrn}");
				
				// Store CaseState into cache.
				var userState = await _userStateCache.GetData(GetUserName()) ?? new UserState(GetUserName());
				userState.TrustUkPrn = trustUkPrn;
				userState.CreateCaseModel = new CreateCaseModel();
				
				await _userStateCache.StoreData(GetUserName(), userState);

				return new JsonResult(new { redirectUrl = Url.Page("Concern/Index") });
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::IndexPageModel::OnGetSelectedTrust::Exception - {Message}", ex.Message);
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}