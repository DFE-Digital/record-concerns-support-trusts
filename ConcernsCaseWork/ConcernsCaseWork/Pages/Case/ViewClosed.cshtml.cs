using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ViewClosedPageModel : AbstractPageModel
	{
		private readonly IRecordModelService _recordModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly IActionsModelService _actionsModelService;
		private readonly ILogger<ViewClosedPageModel> _logger;
		private readonly TelemetryClient _telemetryClient;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public List<ActionSummaryModel> CaseActions { get; private set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.ClosedCasesSummaryPage);

		public string CaseArchivePassword { get; set; }
		
		public ViewClosedPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			IRecordModelService recordModelService,
			IActionsModelService actionsModelService,
			ILogger<ViewClosedPageModel> logger,
			TelemetryClient telemetryClient,
			IOptions<SiteOptions> siteOptions)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_recordModelService = Guard.Against.Null(recordModelService);
			_actionsModelService = Guard.Against.Null(actionsModelService);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
			CaseArchivePassword = siteOptions.Value.CaseArchivePassword;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var caseUrn = GetRequestedCaseUrn();
				var userName = GetUserName();

				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);

				if (CaseModel.IsOpen())
				{
					_logger.LogInformation("Redirecting to /case/{CaseUrn}/management as this case is open", CaseModel.Urn);
					return Redirect($"/case/{CaseModel.Urn}/management");
				}
				
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(caseUrn);
				CaseModel.RecordsModel = recordsModel;

				CaseActions = (await _actionsModelService.GetActionsSummary(caseUrn)).ClosedActions;
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "VIEW CLOSED",
					EventDescription = "Viewing closed cases",
					EventPayloadJson = "",
					EventUserName = userName
				});
				
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{EchoCallerName}::Exception - {Message}", 
					nameof(ViewClosedPageModel), LoggingHelpers.EchoCallerName(), ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}
		
		private long GetRequestedCaseUrn()
		{
			var caseUrnValue = GetRouteValue("urn");

			if (caseUrnValue == null || !long.TryParse(caseUrnValue, out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			return caseUrn;
		}

		private string GetUserName() => User.Identity?.Name;
	}
}