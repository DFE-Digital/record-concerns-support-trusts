using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectCaseTypePageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectCaseTypePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }
		
	[BindProperty]
	[Required(ErrorMessage = "Case Type must be selected")]
	public CaseTypes? CaseType { get; set; }
	
	public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

	public SelectCaseTypePageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectCaseTypePageModel> logger,
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
			await SetTrustAddress();
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
			if (!ModelState.IsValid)
			{
				return Page();
			}
			
			await ResetUserState();

			switch (CaseType)
			{
				case CaseTypes.Concern:
					return Redirect("/case/concern/index");
				case CaseTypes.NonConcern:
					return Redirect("/case/create/nonconcerns");
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

	private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);

	public enum CaseTypes
	{
		Concern,
		NonConcern
	}
}