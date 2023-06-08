using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

		[BindProperty]
		public TextAreaUiComponent Issue { get; set; }

		[BindProperty]
		public TextAreaUiComponent CurrentStatus { get; set; }

		[BindProperty]
		public TextAreaUiComponent CaseAim { get; set; }

		[BindProperty]
		public TextAreaUiComponent DeEscalationPoint { get; set; }

		[BindProperty]
		public TextAreaUiComponent NextSteps { get; set; }

		[BindProperty]
		public TextAreaUiComponent CaseHistory { get; set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int? CaseUrn { get; set; }

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
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				// Fetch UI data
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}
				
				// Complete create case model
				var userState = await GetUserState();
				
				var createCaseModel = userState.CreateCaseModel;
				createCaseModel.Issue = Issue.Text.StringContents;
				createCaseModel.CurrentStatus = CurrentStatus.Text.StringContents;
				createCaseModel.NextSteps = NextSteps.Text.StringContents;
				createCaseModel.CaseAim = CaseAim.Text.StringContents;
				createCaseModel.DeEscalationPoint = DeEscalationPoint.Text.StringContents;
				createCaseModel.CaseHistory = CaseHistory.Text.StringContents;
				
				if (CaseUrn.HasValue)
				{
					return await UpdateCase(createCaseModel);
				}

				return await CreateNewCase(createCaseModel, userState);
				
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
		
		private async Task LoadPage()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;

			if (string.IsNullOrEmpty(trustUkPrn))
				throw new Exception("Cache TrustUkprn is null");
		
			CreateCaseModel = userState.CreateCaseModel;
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			AppInsightsHelper.LogEvent(_telemetry, new AppInsightsModel()
			{
				EventName = "CREATE CASE",
				EventDescription = "Loading the page",
				EventPayloadJson = "",
				EventUserName = userState.UserName
			});

			Issue = CaseComponentBuilder.BuildIssue(nameof(Issue), Issue?.Text.StringContents);
			CurrentStatus = CaseComponentBuilder.BuildCurrentStatus(nameof(CurrentStatus), CurrentStatus?.Text.StringContents);
			CaseAim = CaseComponentBuilder.BuildCaseAim(nameof(CaseAim), CaseAim?.Text.StringContents);
			DeEscalationPoint = CaseComponentBuilder.BuildDeEscalationPoint(nameof(DeEscalationPoint), DeEscalationPoint?.Text.StringContents);
			NextSteps = CaseComponentBuilder.BuildNextSteps(nameof(NextSteps), NextSteps?.Text.StringContents);
			CaseHistory = CaseComponentBuilder.BuildCaseHistory(nameof(CaseHistory), CaseHistory?.Text.StringContents);
		}

		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(User.Identity?.Name);
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}

		private async Task<IActionResult> CreateNewCase(CreateCaseModel model, UserState userState)
		{
			// get the trust being used for the case
			var trust = await _trustService.GetTrustByUkPrn(userState.TrustUkPrn);

			model.TrustUkPrn = trust.GiasData.UkPrn;
			model.TrustCompaniesHouseNumber = trust.GiasData.CompaniesHouseNumber;

			var caseUrn = await _caseModelService.PostCase(model);
			LogToAppInsights("CREATE CASE", $"Case created {caseUrn}", JsonSerializer.Serialize(model), userState.UserName);

			return RedirectToPage("management/index", new { urn = caseUrn });
		}

		private async Task<IActionResult> UpdateCase(CreateCaseModel createCaseModel)
		{
			createCaseModel.DirectionOfTravel = DirectionOfTravelEnum.Deteriorating.ToString();
			await _caseModelService.PatchCase(CaseUrn.Value, createCaseModel);

			return RedirectToPage("management/index", new { urn = CaseUrn });
		}

		private void LogToAppInsights(string eventName, string description, string payload, string userName)
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