using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class SelectTerritoryPageModel : AbstractPageModel
	{
		private readonly ILogger<SelectTerritoryPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private TelemetryClient _telemetryClient;

		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public CreateCaseModel CreateCaseModel { get; private set; }
		
		[BindProperty]
		[Required(ErrorMessage = "An SFSO Territory must be selected")]
		public Territory? Territory { get; set; }

		public SelectTerritoryPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			ILogger<SelectTerritoryPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_telemetryClient = Guard.Against.Null(telemetryClient);
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();
				
			try
			{
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();
				
			try
			{
				if (!ModelState.IsValid)
				{
					return await LoadPage();
				}
				
				var userState = await GetUserState();
				userState.CreateCaseModel.Territory = Territory;
				await _userStateCache.StoreData(GetUserName(), userState);
				
				return RedirectToPage("details");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Page();
		}
		
		public async Task<ActionResult> OnGetCancel()
		{
			try
			{
				UserState userState;
				try
				{
					userState = await GetUserState();
				}
				catch (Exception)
				{
					userState = new UserState(GetUserName());
				}

				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(GetUserName(), userState);
				
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
	
				return Redirect("/");
			}
		}
		
		private async Task<ActionResult> LoadPage()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;

			if (string.IsNullOrEmpty(trustUkPrn))
				throw new Exception("Cache TrustUkprn is null");
		
			CreateCaseModel = userState.CreateCaseModel;
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			_telemetryClient.TrackEvent($"CREATE CASE: {userState.UserName} accessing territory page");
			return Page();
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(GetUserName());
			if (userState is null)
				throw new Exception("Could not retrieve cached new case data for user");
			
			return userState;
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}