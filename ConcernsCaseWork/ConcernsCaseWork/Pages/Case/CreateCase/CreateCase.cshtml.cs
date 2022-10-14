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

	private const int SearchQueryMinLength = 3;
	
	[TempData]
	public int CurrentStep { get; set; }

	[BindProperty(SupportsGet = true)]
	public TrustAddressModel TrustAddress { get; set; }
		
	[BindProperty]
	[TempData]
	public int CaseType { get; set; }

	private void NextStep()
	{
		if (CurrentStep >= 3)
		{
			CurrentStep = 0;
		}
		else
		{
			CurrentStep++;
		}
	}

	public CreateCasePageModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedService,
		ILogger<CreateCasePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);

		if (CurrentStep >= 3)
		{
			CurrentStep = 0;
		}
	}

	public async Task OnGet()
	{
		_logger.LogMethodEntered();
		
		try
		{
			switch (CurrentStep)
			{
				case 0:
				case 1:
					break;
				case 2:
					await OnGetChooseCaseTypeAsync();
					break;
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
  
			TempData["Error.Message"] = ErrorOnGetPage;
		}
	}
	
	public async Task<ActionResult> OnPostAsync()
	{
		_logger.LogMethodEntered();
			
		try
		{
			switch (CurrentStep)        
			{
				case 0:
				case 1:
				case 2:
				case 3:
					return await OnPostChooseCaseTypeAsync();
				case 4:
					break;
			}

			return Page();
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			TempData["Error.Message"] = ErrorOnPostPage;
			
			return RedirectToPage("choosecasetype");
		}
	}
	
	#region Step1 Get Trusts
	
	public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
	{
		_logger.LogMethodEntered();
		
		try
		{
			// Double check search query.
			if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < SearchQueryMinLength)
				return new JsonResult(Array.Empty<TrustSearchModel>());

			var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
			var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);
			
			NextStep();
			
			return new JsonResult(trustSearchResponse);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
		}
	}

	public async Task<ActionResult> OnGetSelectedTrust(string trustUkPrn)
	{
		try
		{
			_logger.LogMethodEntered();

			// Double check selected trust.
			if (string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains("-") || trustUkPrn.Length < SearchQueryMinLength)
				throw new Exception($"Selected trust is incorrect - {trustUkPrn}");

			var userState = await _cachedService.GetData(User.Identity?.Name);
			userState.TrustUkPrn = trustUkPrn;
			await _cachedService.StoreData(User.Identity?.Name, userState);
			
			NextStep();
			
			return new JsonResult(new { redirectUrl = "/case/create" });
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
		}
	}
	
	#endregion
	
	#region Step2 Choose Case Type
			
		public async Task OnGetChooseCaseTypeAsync()
		{
			_logger.LogMethodEntered();

			await SetTrustAddress();
			
			NextStep();
		}

		public async Task<ActionResult> OnPostChooseCaseTypeAsync()
		{
			_logger.LogMethodEntered();
			
			try
			{
				if (!ModelState.IsValid)
				{
					await SetTrustAddress();
					return Page();
				}
				
				// reset user state
				var userState = await _cachedService.GetData(User.Identity?.Name);
				userState.CreateCaseModel = new CreateCaseModel();
				
				await _cachedService.StoreData(User.Identity?.Name, userState);
			
				NextStep();
				
				return RedirectToPage("/case/Concern/Index");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
		}
		
	#endregion
	
	private async Task SetTrustAddress()
	{
		var userState = await _cachedService.GetData(User.Identity?.Name);
		TrustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);
	}
}