using Ardalis.GuardClauses;
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
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private TelemetryClient _telemetryClient;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		
		public AddPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			ILogger<AddPageModel> logger,
			TelemetryClient telemetryClient)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::Concern::AddPageModel::OnGetAsync");
				
			// Fetch UI data
			await LoadPage();
		}
		
		public async Task<ActionResult> OnGetCancel()
		{
			try
			{
				var userState = await GetUserState();
				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(User.Identity?.Name, userState);
				
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::AddPageModel::OnGetCancel::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private async Task LoadPage()
		{
			try
			{
				var userState = await GetUserState();
				var trustUkPrn = userState.TrustUkPrn;
			
				if (string.IsNullOrEmpty(trustUkPrn))
					throw new Exception("Cache TrustUkprn is null");
			
				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				_telemetryClient.TrackEvent($"CREATE CASE: Concerns user {userState.UserName} adding a concern");
				Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::AddPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				Page();
			}
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(User.Identity.Name);
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}
	}
}