using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
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
public class SelectCaseTypePageModel : AbstractPageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectCaseTypePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	[BindProperty]
	public RadioButtonsUiComponent CaseType { get; set; }

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
			
			await CreateCasePlaceholder();

			var selectedCaseType = (CaseType)CaseType.SelectedId;

			switch (selectedCaseType)
			{
				case API.Contracts.Case.CaseType.Concerns:
					return Redirect("/case/concern");
				case API.Contracts.Case.CaseType.NonConcerns:
					return Redirect("/case/territory");
				default:
					throw new Exception($"Unrecognised case type {selectedCaseType}");
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			SetErrorMessage(ErrorOnPostPage);
		}
		
		return Page();
	}

	private RadioButtonsUiComponent BuildCaseTypeComponent(int? selectedId = null)
	{
		var enumValues = new[]
		{
			new { CaseType = API.Contracts.Case.CaseType.Concerns, HintText = "This includes narrative, actions or decisions related to any new concern(s)." },
			new { CaseType = API.Contracts.Case.CaseType.NonConcerns, HintText = "This includes proactive SRMA (School Resource Management Adviser), TFF (Trust Financial Forecast) activity and decisions." }
		};

		var radioItems = enumValues.Select(v =>
		{
			return new SimpleRadioItem(v.CaseType.Description(), (int)v.CaseType) { TestId = v.CaseType.ToString(), HintText = v.HintText };
		}).ToArray();

		return new(ElementRootId: "case-type", Name: nameof(CaseType), "What are you recording?")
		{
			RadioItems = radioItems,
			SelectedId = selectedId,
			Required = true,
			DisplayName = "case type"
		};
	}

	private void LoadPageComponents()
	{
		CaseType = BuildCaseTypeComponent(CaseType?.SelectedId);
	}

	private async Task CreateCasePlaceholder()
	{
		var userName = GetUserName();
		var userState = await _cachedUserService.GetData(userName);
		userState.CreateCaseModel = new CreateCaseModel();
		await _cachedUserService.StoreData(userName, userState);
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