using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectTrustPageModel : CreateCaseBasePageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectTrustPageModel> _logger;
	private const int _searchQueryMinLength = 3;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	[FromQuery(Name = "step")]
	public CreateCaseSteps CreateCaseStep { get; set; } = CreateCaseSteps.SearchForTrust;

	[BindProperty]
	public FindTrustModel FindTrustModel { get; set; }

	public SelectTrustPageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectTrustPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper) : base(cachedUserService, claimsPrincipalHelper)
	{
		_trustModelService = trustModelService;
		_cachedUserService = cachedUserService;
		_logger = logger;
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

			SetErrorMessage(ErrorOnGetPage);
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

			return RedirectToPage("SelectCaseDivision");
		}
		catch (Exception ex)
		{
			return HandleExceptionForAjaxCall(ex);
		}

		async Task CacheTrustUkPrn()
		{
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName) ??  new UserState(GetUserName());
			userState.TrustUkPrn = FindTrustModel.SelectedTrustUkprn;
			userState.TeamLeader = null;
			await _cachedUserService.StoreData(userName, userState);
		}

		void SetNextStep() => CreateCaseStep = CreateCaseSteps.SelectCaseType;
	}

	private bool TrustUkPrnIsValid() => !(string.IsNullOrEmpty(FindTrustModel.SelectedTrustUkprn) || FindTrustModel.SelectedTrustUkprn.Contains('-') || FindTrustModel.SelectedTrustUkprn.Length < _searchQueryMinLength);

	private async Task SetTrustAddress()
	{
		var userName = GetUserName();
		var userState = await GetUserState();

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