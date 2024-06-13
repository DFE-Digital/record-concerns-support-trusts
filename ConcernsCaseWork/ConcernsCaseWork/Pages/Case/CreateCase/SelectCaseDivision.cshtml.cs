using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectCaseDivisionPageModel : CreateCaseBasePageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly IUserStateCachedService _cachedUserService;
	private readonly ILogger<SelectCaseDivisionPageModel> _logger;
	private readonly IFeatureManager _featureManager;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	[BindProperty]
	public RadioButtonsUiComponent CaseDivision { get; set; }

	public SelectCaseDivisionPageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectCaseDivisionPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper,
		IFeatureManager featureManager) : base(cachedUserService, claimsPrincipalHelper)
	{
		_trustModelService = trustModelService;
		_cachedUserService = cachedUserService;
		_logger = logger;
		_featureManager = featureManager;
	}

	public async Task<IActionResult> OnGet()
	{
		_logger.LogMethodEntered();
		
		try
		{
			await LoadPage();
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
				await LoadPage();

				return Page();
			}
			
			var selectedCaseDivision = (Division)CaseDivision.SelectedId;

			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			userState.CreateCaseModel = new CreateCaseModel();
			userState.CreateCaseModel.Division = selectedCaseDivision;

			await _cachedUserService.StoreData(userName, userState);

			switch (selectedCaseDivision)
			{
				case Division.SFSO:
					return Redirect("/case/territory");
				case Division.RegionsGroup:
					return Redirect("/case/create/region");
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

	private async Task <RadioButtonsUiComponent> BuildCaseManagerComponent(int? selectedId = null)
	{
		var radioItems = new List<SimpleRadioItem>()
		{
			new SimpleRadioItem("SFSO (Schools Financial Support and Oversight)", (int)Division.SFSO) { TestId = Division.SFSO.ToString() },
			new SimpleRadioItem("Regions Group", (int)Division.RegionsGroup) { TestId = Division.RegionsGroup.ToString() },
		};

		return new(ElementRootId: "case-division", Name: nameof(CaseDivision), "Who is managing this case?")
		{
			RadioItems = radioItems,
			SelectedId = selectedId,
			Required = true,
			DisplayName = "case division"
		};
	}

	private async Task LoadPage()
	{
		var userState = await GetUserState();
		TrustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);

		CaseDivision = await BuildCaseManagerComponent(CaseDivision?.SelectedId);
	}
}