using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectTrustPageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectTrustPageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private const int _searchQueryMinLength = 3;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

	public SelectTrustPageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectTrustPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_cachedUserService = Guard.Against.Null(cachedUserService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}

	// This is an AJAX call
	public async Task<IActionResult> OnGetTrustsSearchResult(string searchQuery)
	{
		_logger.LogMethodEntered();
		
		try
		{
			if (!SearchQueryIsValid()) 
				return new JsonResult(new List<TrustSearchModel>());

			var trustSearchResponse = await BuildTrustResponse();
			
			return new JsonResult(trustSearchResponse);
		}
		catch (Exception ex)
		{
			return HandleExceptionForAjaxCall(ex);
		}
		
		bool SearchQueryIsValid() => !(string.IsNullOrEmpty(searchQuery) || searchQuery.Length < _searchQueryMinLength);

		async Task<IList<TrustSearchModel>> BuildTrustResponse()
		{
			var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
			return await _trustModelService.GetTrustsBySearchCriteria(trustSearch);
		}
	}

	// This is an AJAX call
	public async Task<IActionResult> OnGetSelectedTrust(string trustUkPrn)
	{
		_logger.LogMethodEntered();
		
		try
		{
			if (!TrustUkPrnIsValid())
				throw new Exception($"Selected trust is incorrect - {trustUkPrn}");

			await CacheTrustUkPrn();

			return BuildRedirectResponse();
		}
		catch (Exception ex)
		{
			return HandleExceptionForAjaxCall(ex);
		}
		
		bool TrustUkPrnIsValid() => !(string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains('-') || trustUkPrn.Length < _searchQueryMinLength);

		async Task CacheTrustUkPrn()
		{
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName) ?? new UserState(userName);
			userState.TrustUkPrn = trustUkPrn;
			await _cachedUserService.StoreData(userName, userState);
		}
		
		JsonResult BuildRedirectResponse() => new (new { redirectUrl = "/case/create/type" });
	}

	private ObjectResult HandleExceptionForAjaxCall(Exception ex)
	{
		_logger.LogErrorMsg(ex);
			
		return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
	}
	
	private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
}