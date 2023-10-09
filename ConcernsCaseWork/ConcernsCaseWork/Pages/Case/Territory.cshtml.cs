using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class SelectTerritoryPageModel : CreateCaseBasePageModel
	{
		private readonly ILogger<SelectTerritoryPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private TelemetryClient _telemetryClient;

		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public CreateCaseModel CreateCaseModel { get; private set; }

		[BindProperty]
		public RadioButtonsUiComponent Territory { get; set; }

		public SelectTerritoryPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			ILogger<SelectTerritoryPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient) : base(userStateCache, claimsPrincipalHelper)
		{
			_trustModelService = trustModelService;
			_userStateCache = userStateCache;
			_logger = logger;
			_telemetryClient = telemetryClient;
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
				SetErrorMessage(ErrorOnGetPage);
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
				userState.CreateCaseModel.Territory = (Territory)Territory.SelectedId;
				await _userStateCache.StoreData(GetUserName(), userState);

				return Redirect("/case/create/type");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}
			
			return Page();
		}
		
		public async Task<ActionResult> OnGetCancel()
		{
			_logger.LogMethodEntered();

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
		
			CreateCaseModel = userState.CreateCaseModel;
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
			{
				EventName = "CREATE CASE",
				EventDescription = "Accessing territory page",
				EventPayloadJson = "",
				EventUserName = userState.UserName
			});

			Territory = CaseComponentBuilder.BuildTerritory(nameof(Territory), Territory?.SelectedId);

			return Page();
		}
	}
}