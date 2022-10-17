using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using Service.Redis.Users;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CreateCasePageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedService;
	private readonly ILogger<CreateCasePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private const int _searchQueryMinLength = 3;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }
		
	[BindProperty]
	public CaseTypes CaseType { get; set; }

	[TempData]
	public CreateCaseSteps CreateCaseStep { get; set; } = CreateCaseSteps.SearchForTrust;

	public CreateCasePageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedService,
		ILogger<CreateCasePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}

	public async Task OnGet()
	{
		_logger.LogMethodEntered();
		
		try
		{
			if (CreateCaseStep == CreateCaseSteps.SelectCaseType)
			{
				await SetTrustAddress();
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
  
			TempData["Error.Message"] = ErrorOnGetPage;
		}
	}
	
	public async Task<ActionResult> OnPost()
	{
		_logger.LogMethodEntered();
			
		try
		{
			if (CreateCaseStep != CreateCaseSteps.SelectCaseType)
			{
				throw new Exception();
			}

			if (!ModelState.IsValid)
			{
				await SetTrustAddress();
				return Page();
			}

			await ResetUserState();
			ResetCurrentStep();
			
			return CaseType switch
			{
				CaseTypes.Concern => Redirect("/case/Concern/Index"),
				CaseTypes.NonConcern => Redirect("/case/create/nonconcerns"),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			TempData["Error.Message"] = ErrorOnPostPage;
		}
		
		return Page();
		
		async Task ResetUserState()
		{
			var userState = await _cachedService.GetData(User.Identity?.Name);
			userState.CreateCaseModel = new CreateCaseModel();
			await _cachedService.StoreData(User.Identity?.Name, userState);
		}
		
		void ResetCurrentStep()
		{
			CreateCaseStep = CreateCaseSteps.SearchForTrust;
		}
	}
	
	// This is an AJAX call
	public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
	{
		_logger.LogMethodEntered();
		
		try
		{
			if (!SearchQueryIsValid()) 
				return new JsonResult(Array.Empty<TrustSearchModel>());

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
	public async Task<ActionResult> OnGetSelectedTrust(string trustUkPrn)
	{
		_logger.LogMethodEntered();
		
		try
		{
			if (!TrustUkPrnIsValid())
				throw new Exception($"Selected trust is incorrect - {trustUkPrn}");

			await CacheTrustUkPrn();
			
			SetNextStep();

			return ReloadPageForNextStep();
		}
		catch (Exception ex)
		{
			return HandleExceptionForAjaxCall(ex);
		}
		
		bool TrustUkPrnIsValid() => !(string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains('-') || trustUkPrn.Length < _searchQueryMinLength);

		async Task CacheTrustUkPrn()
		{
			var userState = await _cachedService.GetData(User.Identity?.Name);
			userState.TrustUkPrn = trustUkPrn;
			await _cachedService.StoreData(User.Identity?.Name, userState);
		}
		
		void SetNextStep() => CreateCaseStep = CreateCaseSteps.SelectCaseType;
		
		JsonResult ReloadPageForNextStep() => new (new { redirectUrl = "/case/create" });
	}

	private async Task SetTrustAddress()
	{
		var userState = await _cachedService.GetData(User.Identity?.Name);
		TrustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);
	}

	private ObjectResult HandleExceptionForAjaxCall(Exception ex)
	{
		_logger.LogErrorMsg(ex);
			
		return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
	}
	
	public enum CaseTypes
	{
		Concern,
		NonConcern
	}
	
	public enum CreateCaseSteps
	{
		SearchForTrust,
		SelectCaseType
	}
}