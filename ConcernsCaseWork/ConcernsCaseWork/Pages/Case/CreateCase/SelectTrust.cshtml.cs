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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

	[FromQuery(Name = "step")]
	public CreateCaseSteps CreateCaseStep { get; set; } = CreateCaseSteps.SearchForTrust;

	[BindProperty]
	public FindTrustModel FindTrustModel { get; set; }

	public SelectTrustPageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectTrustPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_cachedUserService = Guard.Against.Null(cachedUserService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		FindTrustModel = new();
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
			else
			{
				ResetCurrentStep();
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			TempData["Error.Message"] = ErrorOnGetPage;
		}

		return Page();
	}

	private void ResetCurrentStep()
	{
		CreateCaseStep = CreateCaseSteps.SearchForTrust;
	}

	public async Task<ActionResult> OnPostSelectedTrust()
	{
		_logger.LogMethodEntered();

		try
		{
			if (!ModelState.IsValid)
			{
				TempData["Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				return Page();
			}

			if (!TrustUkPrnIsValid())
			{
				throw new Exception($"Selected trust is incorrect - {FindTrustModel.SelectedTrustUkprn}");
			}

			await CacheTrustUkPrn();

			SetNextStep();

			return RedirectToPage("SelectCaseType");
		}
		catch (Exception ex)
		{
			return HandleExceptionForAjaxCall(ex);
		}


		async Task CacheTrustUkPrn()
		{
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			userState.TrustUkPrn = FindTrustModel.SelectedTrustUkprn;
			await _cachedUserService.StoreData(userName, userState);
		}

		void SetNextStep() => CreateCaseStep = CreateCaseSteps.SelectCaseType;
	}

	private bool TrustUkPrnIsValid() => !(string.IsNullOrEmpty(FindTrustModel.SelectedTrustUkprn) || FindTrustModel.SelectedTrustUkprn.Contains('-') || FindTrustModel.SelectedTrustUkprn.Length < _searchQueryMinLength);

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

	async Task<UserState> GetUserState() => await _cachedUserService.GetData(GetUserName());

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