using Ardalis.GuardClauses;
using Ardalis.GuardClauses;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		private readonly ITrustService _trustService;
		private readonly IUserStateCachedService _userStateCache;
		private TelemetryClient _telemetry;
		
		public CreateCaseModel CreateCaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public bool IsAddtoCase { get; private set; }
		
		public DetailsPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			IUserStateCachedService userStateCache, 
			ILogger<DetailsPageModel> logger,
			TelemetryClient telemetryClient,
			ITrustService trustService)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_caseModelService = Guard.Against.Null(caseModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_trustService = Guard.Against.Null(trustService);
			_telemetry = Guard.Against.Null(telemetryClient);
			
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::DetailsPageModel::OnGetAsync");
			
			// Fetch UI data
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue != null)
			{
				IsAddtoCase = true;
			}
			await LoadPage();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsPageModel::OnPostAsync");
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue != null)
				{
					IsAddtoCase = true;
				}
				if (IsAddtoCase)
				{
					if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
						throw new Exception("CaseUrn is null or invalid to parse");
					await UpdateCase(caseUrn);
					return RedirectToPage("management/index", new { urn = caseUrn });
				}
				else
				{
					var caseUrn =await CreateNewCase();
					return RedirectToPage("management/index", new { urn = caseUrn });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::DetailsPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return await LoadPage();
		}

		private async Task<long> CreateNewCase()
		{
			var issue = Request.Form["issue"];
			var currentStatus = Request.Form["current-status"];
			var nextSteps = Request.Form["next-steps"];
			var caseAim = Request.Form["case-aim"];
			var deEscalationPoint = Request.Form["de-escalation-point"];
			var caseHistory = Request.Form["case-history"];

			if (string.IsNullOrEmpty(issue)) 
				throw new Exception("Missing form values");
				
			// Complete create case model
			var userState = await GetUserState();
				
			// get the trust being used for the case
			var trust = await this._trustService.GetTrustByUkPrn(userState.TrustUkPrn);
				
			var createCaseModel = userState.CreateCaseModel;
			createCaseModel.Issue = issue;
			createCaseModel.CurrentStatus = currentStatus;
			createCaseModel.NextSteps = nextSteps;
			createCaseModel.CaseAim = caseAim;
			createCaseModel.DeEscalationPoint = deEscalationPoint;
			createCaseModel.TrustUkPrn = trust.GiasData.UkPrn;
			createCaseModel.CaseHistory = caseHistory;
			createCaseModel.TrustCompaniesHouseNumber = trust.GiasData.CompaniesHouseNumber;
				
			var caseUrn = await _caseModelService.PostCase(createCaseModel);
			await LogToAppInsights("CREATE CASE", $"Case created {caseUrn}",JsonSerializer.Serialize(createCaseModel),userState.UserName);
			return caseUrn;
		}

		private async Task UpdateCase(long caseUrn)
		{
			var issue = Request.Form["issue"];
			var currentStatus = Request.Form["current-status"];
			var nextSteps = Request.Form["next-steps"];
			var caseAim = Request.Form["case-aim"];
			var deEscalationPoint = Request.Form["de-escalation-point"];
			var caseHistory = Request.Form["case-history"];

			if (string.IsNullOrEmpty(issue)) 
				throw new Exception("Missing form values");
				
			// Complete create case model
			var userState = await GetUserState();
				
			// get the trust being used for the case
			//var trust = await this._trustService.GetTrustByUkPrn(userState.TrustUkPrn);
			//var current = await _caseModelService.GetCaseByUrn(caseUrn);	
			var createCaseModel = userState.CreateCaseModel;
			createCaseModel.Issue = issue;
			createCaseModel.CurrentStatus = currentStatus;
			createCaseModel.NextSteps = nextSteps;
			createCaseModel.CaseAim = caseAim;
			createCaseModel.DeEscalationPoint = deEscalationPoint;
			createCaseModel.CaseHistory = caseHistory;
			caseUrn = await _caseModelService.PatchCase((int)caseUrn,createCaseModel);
		}

		
		private async Task<ActionResult> LoadPage()
		{
			try
			{
				var userState = await GetUserState();
				var trustUkPrn = userState.TrustUkPrn;
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue != null)
				{
					IsAddtoCase = true;
				}
				if (string.IsNullOrEmpty(trustUkPrn))
					throw new Exception("Cache TrustUkprn is null");
		
				CreateCaseModel = userState.CreateCaseModel;
				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				await LogToAppInsights("CREATE CASE","Loading the page","",userState.UserName);
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::DetailsPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(User.Identity?.Name);
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}

		private async Task LogToAppInsights(string eventName, string description, string payload, string userName)
		{
			AppInsightsHelper.LogEvent(_telemetry, new AppInsightsModel()
			{
				EventName = eventName,
				EventDescription = description,
				EventPayloadJson = payload,
				EventUserName = userName
			});
		}
	}
}