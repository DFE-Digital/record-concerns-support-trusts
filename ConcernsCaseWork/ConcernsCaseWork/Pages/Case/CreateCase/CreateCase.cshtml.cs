using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
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
public class CreateCasePageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<CreateCasePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private const int _searchQueryMinLength = 3;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	[BindProperty]
	public CaseTypes CaseType { get; set; }

	[TempData]
	public CreateCaseSteps CreateCaseStep { get; set; } = CreateCaseSteps.SearchForTrust;

	[BindProperty]
	public FindTrustModel FindTrustModel { get; set; }

	public CreateCasePageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<CreateCasePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_cachedUserService = Guard.Against.Null(cachedUserService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}

	public async Task<IActionResult> OnGet()
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

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		_logger.LogMethodEntered();

		try
		{
			if (CreateCaseStep != CreateCaseSteps.SelectCaseType)
			{
				throw new Exception();
			}

			await ResetUserState();
			ResetCurrentStep();

			switch (CaseType)
			{
				case CaseTypes.Concern:
					return Redirect("/case/concern/index");
				case CaseTypes.NonConcern:
					return Redirect("/case/create/nonconcerns");
				case CaseTypes.NotSelected:
				default:
					throw new ArgumentOutOfRangeException(nameof(CaseType));
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			TempData["Error.Message"] = ErrorOnPostPage;
		}

		return Page();

		async Task ResetUserState()
		{
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			userState.CreateCaseModel = new CreateCaseModel();
			await _cachedUserService.StoreData(userName, userState);
		}

		void ResetCurrentStep()
		{
			CreateCaseStep = CreateCaseSteps.SearchForTrust;
		}
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
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			userState.TrustUkPrn = trustUkPrn;
			await _cachedUserService.StoreData(userName, userState);
		}

		void SetNextStep() => CreateCaseStep = CreateCaseSteps.SelectCaseType;

		JsonResult ReloadPageForNextStep() => new (new { redirectUrl = "/case/create" });
	}

	private async Task SetTrustAddress()
	{
		var userName = GetUserName();
		var userState = await _cachedUserService.GetData(userName);
		if (userState == null)
		{
			throw new Exception($"Could not retrieve cache for user '{userName}'");
		}

		if (string.IsNullOrEmpty(userState.TrustUkPrn))
		{
			throw new Exception($"Could not retrieve trust from cache for user '{userName}'");
		}

		var trustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);

		TrustAddress = trustAddress ?? throw new Exception($"Could not find trust with UK PRN of {userState.TrustUkPrn}");
	}

	private ObjectResult HandleExceptionForAjaxCall(Exception ex)
	{
		_logger.LogErrorMsg(ex);

		return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
	}

	private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);

	public enum CaseTypes
	{
		NotSelected,
		Concern,
		NonConcern
	}

	public enum CreateCaseSteps
	{
		SearchForTrust,
		SelectCaseType
	}
}