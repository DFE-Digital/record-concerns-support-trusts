using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectCaseTypePageModel : CreateCaseBasePageModel
{
	private readonly ITrustModelService _trustModelService;
	private readonly ILogger<SelectCaseTypePageModel> _logger;

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }

	public CreateCaseModel CreateCaseModel { get; private set; }

	[BindProperty]
	public RadioButtonsUiComponent CaseType { get; set; }

	public SelectCaseTypePageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		ILogger<SelectCaseTypePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper) : base(cachedUserService, claimsPrincipalHelper)
	{
		_trustModelService = trustModelService;
		_logger = logger;
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
			
			var selectedCaseType = (CaseType)CaseType.SelectedId;

			switch (selectedCaseType)
			{
				case API.Contracts.Case.CaseType.Concerns:
					return Redirect("/case/concern");
				case API.Contracts.Case.CaseType.NonConcerns:
					return Redirect("/case/create/nonconcerns/details");
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

	private async Task LoadPage()
	{
		var userState = await GetUserState();
		TrustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);
		CreateCaseModel = userState.CreateCaseModel;

		CaseType = BuildCaseTypeComponent(CaseType?.SelectedId);
	}
}