using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectCaseDivisionPageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectCaseDivisionPageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	[BindProperty]
	public RadioButtonsUiComponent CaseDivision { get; set; }

	public SelectCaseDivisionPageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectCaseDivisionPageModel> logger,
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
			LoadPageComponents();
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			SetErrorMessage(ErrorOnGetPage);
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
				await SetTrustAddress();
				LoadPageComponents();

				return Page();
			}
			
			var selectedCaseDivision = (Division)CaseDivision.SelectedId;

			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			userState.CreateCaseModel = new CreateCaseModel();
			userState.CreateCaseModel.Division = selectedCaseDivision;

			await _cachedUserService.StoreData(userName, userState);

			//return RedirectToPage("SelectCaseType");
			
			switch (selectedCaseDivision)
			{
				case Division.SFSO:
					return Redirect("/case/territory");
				case Division.RegionsGroup:
					return Redirect("/case/area");
				default:
					throw new Exception($"Unrecognised case manager {selectedCaseDivision}");
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			SetErrorMessage(ErrorOnPostPage);
		}
		
		return Page();
	}

	private RadioButtonsUiComponent BuildCaseManagerComponent(int? selectedId = null)
	{
		var enumValues = new[]
		{
			new { CaseDivision = API.Contracts.Case.Division.SFSO },
			new { CaseDivision = API.Contracts.Case.Division.RegionsGroup }
		};

		var radioItems = enumValues.Select(v =>
		{
			return new SimpleRadioItem(v.CaseDivision.Description(), (int)v.CaseDivision) { TestId = v.CaseDivision.ToString(), Disabled = v.CaseDivision == Division.RegionsGroup };
		}).ToArray();


		return new(ElementRootId: "case-division", Name: nameof(CaseDivision), "Who is managing this case?")
		{
			RadioItems = radioItems,
			SelectedId = selectedId,
			Required = true,
			DisplayName = "case division"
		};
	}

	private void LoadPageComponents()
	{
		CaseDivision = BuildCaseManagerComponent(CaseDivision?.SelectedId);
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
}