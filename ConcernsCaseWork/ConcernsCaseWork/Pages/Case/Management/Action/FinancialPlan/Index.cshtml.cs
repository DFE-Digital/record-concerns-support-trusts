﻿using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Service.Permissions;
using Microsoft.CodeAnalysis.Operations;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;

		public FinancialPlanModel FinancialPlanModel { get; set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");
		public bool UserCanDelete { get; private set; }


		public IndexPageModel(IFinancialPlanModelService financialPlanModelService, ICasePermissionsService casePermissionsService, ILogger<IndexPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			long financialPlanId = 0;

			try
			{
				_logger.LogInformation("Case::Action::FinancialPlan::IndexPageModel::OnGetAsync");

				(caseUrn, financialPlanId) = GetRouteData();

				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId);
				UserCanDelete = await _casePermissionsService.UserHasDeletePermissions(caseUrn);

				if (FinancialPlanModel == null)
				{
					throw new Exception("Could not load this Financial Plan");
				}
				
				if (FinancialPlanModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::FinancialPlan::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		private (long caseUrn, long financialPlanId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var financialPlanIdValue = RouteData.Values["financialPlanId"];
			if (financialPlanIdValue == null || !long.TryParse(financialPlanIdValue.ToString(), out long financialPlanId) || financialPlanId == 0)
				throw new Exception("FinancialPlanId is null or invalid to parse");

			return (caseUrn, financialPlanId);
		}
	}
}