using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public List<ActionSummary> CaseActions { get; private set; }
		
		public ViewClosedPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			IRecordModelService recordModelService,
			IActionsModelService actionsModelService,
			ILogger<ViewClosedPageModel> logger)
		{
			_caseModelService = caseModelService;
			_trustModelService = trustModelService;
			_recordModelService = recordModelService;
			_actionsModelService = actionsModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("{ClassName}::{EchoCallerName}", nameof(ViewClosedPageModel), LoggingHelpers.EchoCallerName());

				var caseUrn = GetRequestedCaseUrn();
				var userName = GetUserName();

				CaseModel = await _caseModelService.GetCaseByUrn(userName, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(userName, caseUrn);
				CaseModel.RecordsModel = recordsModel;

				CaseActions = (await _actionsModelService.GetClosedActionsSummary(userName, caseUrn)).ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{EchoCallerName}::Exception - {Message}", 
					nameof(ViewClosedPageModel), LoggingHelpers.EchoCallerName(), ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
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