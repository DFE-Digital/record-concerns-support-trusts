using Ardalis.GuardClauses;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ISRMAService _srmaModelService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly IDecisionService _decisionService;
		private readonly ICaseActionValidator _caseActionValidator;
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<ClosurePageModel> _logger;
		private TelemetryClient _telemetryClient;
		private readonly IUserStateCachedService _userStateCache;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty]
		public TextAreaUiComponent RationaleForClosure { get; set; }

		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

		public ClosurePageModel(
			ICaseModelService caseModelService, 
			ITrustModelService trustModelService, 
			IRecordModelService recordModelService, 
			IStatusCachedService statusCachedService, 
			ISRMAService srmaModelService, 
			IFinancialPlanModelService financialPlanModelService, 
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService, 
			INtiWarningLetterModelService ntiWarningLetterModelService, 
			INtiModelService ntiModelService,
			IDecisionService decisionService,
			ITrustFinancialForecastService trustFinancialForecastService,
			ICaseActionValidator caseActionValidator,
			ILogger<ClosurePageModel> logger,
			IUserStateCachedService cachedService,
			TelemetryClient telemetryClient)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_recordModelService = Guard.Against.Null(recordModelService);
			_statusCachedService = Guard.Against.Null(statusCachedService);
			_srmaModelService = Guard.Against.Null(srmaModelService);
			_financialPlanModelService = Guard.Against.Null(financialPlanModelService);
			_ntiUnderConsiderationModelService = Guard.Against.Null(ntiUnderConsiderationModelService);
			_ntiWarningLetterModelService = Guard.Against.Null(ntiWarningLetterModelService);
			_ntiModelService = Guard.Against.Null(ntiModelService);
			_decisionService = Guard.Against.Null(decisionService);
			_trustFinancialForecastService = Guard.Against.Null(trustFinancialForecastService);
			_caseActionValidator = Guard.Against.Null(caseActionValidator);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
			_userStateCache = Guard.Against.Null(cachedService);
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var validationMessages = await ValidateCloseConcern(CaseId);

				if (validationMessages.Count > 0)
				{
					TempData["OpenActions.Message"] = validationMessages;
					return;
				}

				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosurePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnPostCloseCase()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var userState = await GetUserState();

				if (!(await IsCaseAlreadyClosed(CaseId)))
				{
					var patchCaseModel = new PatchCaseModel
					{
						// Update patch case model
						Urn = CaseId,
						ClosedAt = DateTimeOffset.Now,
						UpdatedAt = DateTimeOffset.Now,
						StatusName = StatusEnum.Close.ToString(),
						ReasonAtReview = RationaleForClosure.Text.StringContents
					};

					AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
					{
						EventName = "CASE CLOSED",
						EventDescription = $"Case has been closed {CaseId}",
						EventPayloadJson = JsonSerializer.Serialize(patchCaseModel),
						EventUserName = userState.UserName
					});

					await _caseModelService.PatchClosure(patchCaseModel);
				}
					
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Redirect("closure");
		}

		private async Task<bool> IsCaseAlreadyClosed(long urn)
		{
			var closedState = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());
			var caseDto = await _caseModelService.GetCaseByUrn(urn);

			return closedState != null && caseDto?.StatusId == closedState?.Id;
		}

		private async Task<List<string>> ValidateCloseConcern(long caseUrn)
		{
			List<string> errorMessages = new List<string>();
			List<CaseActionModel> caseActionModels = new List<CaseActionModel>();

			var recordsModels = await _recordModelService.GetRecordsModelByCaseUrn(caseUrn);
			var liveStatus = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			var numberOfOpenConcerns = recordsModels.Count(r => r.StatusId.CompareTo(liveStatus.Id) == 0);
			
			var srmaModelsTask = _srmaModelService.GetSRMAsForCase(caseUrn);
			var financialPlanModelsTask = _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn);
			var ntiUnderConsiderationModelsTask = _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn);
			var ntiWarningLetterModelsTask = _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn);
			var ntiModelModelsTask = _ntiModelService.GetNtisForCaseAsync(caseUrn);
			var decisionsTask = GetDecisions(caseUrn);
			var trustFinancialForecastTask = _trustFinancialForecastService.GetAllForCase((int)caseUrn);

			caseActionModels.AddRange(await srmaModelsTask);
			caseActionModels.AddRange(await financialPlanModelsTask);
			caseActionModels.AddRange(await ntiUnderConsiderationModelsTask);
			caseActionModels.AddRange(await ntiWarningLetterModelsTask);
			caseActionModels.AddRange(await ntiModelModelsTask);
			caseActionModels.AddRange(await decisionsTask);
			caseActionModels.AddRange((await trustFinancialForecastTask).Select(x => x.ToTrustFinancialForecastSummaryModel()));
			var caseActionErrorMessages = _caseActionValidator.Validate(caseActionModels);

			errorMessages.AddRange(caseActionErrorMessages);

			if (numberOfOpenConcerns > 0)
			{
				errorMessages.Add("Resolve Concerns");
			}

			if (await IsCaseAlreadyClosed(caseUrn))
			{
				errorMessages.Add("This case is already closed.");
			}

			return errorMessages;
		}

		private async Task LoadPage()
		{
			// Fetch UI data
			var caseModel = await _caseModelService.GetCaseByUrn(CaseId);

			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseModel.TrustUkPrn);
			RationaleForClosure = BuildRationaleForClosureComponent(RationaleForClosure?.Text.StringContents);
		}

		private TextAreaUiComponent BuildRationaleForClosureComponent(string contents = "")
		=> new("case-outcomes", nameof(RationaleForClosure), "Rationale for closure")
		{
			Text = new ValidateableString()
			{
				MaxLength = 200,
				StringContents = contents,
				DisplayName = "Rationale for closure",
				Required = true
			}
		};

		private async Task<List<DecisionSummaryModel>> GetDecisions(long caseUrn)
		{
			var apiDecisions = await _decisionService.GetDecisionsByCaseUrn(caseUrn);

			var result = apiDecisions.Select(d => DecisionMapping.ToDecisionSummaryModel(d)).ToList();

			return result;
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