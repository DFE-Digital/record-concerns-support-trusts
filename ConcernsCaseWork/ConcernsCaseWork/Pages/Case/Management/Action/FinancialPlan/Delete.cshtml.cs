using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public string DateOpened { get; set; }

		public DeletePageModel(ICaseModelService caseModelService,
			IFinancialPlanModelService financialPlanModelService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_financialPlanModelService = financialPlanModelService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long decisionId = 0;
			
			try
			{
				_logger.LogMethodEntered();
				(caseUrn, decisionId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, decisionId);
		}

		public async Task<ActionResult> OnGetDeleteFinancialPlan()
		{
			long caseUrn = 0;
			long financialPlanId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, financialPlanId) = GetRouteData();
				
				await _financialPlanModelService.DeleteFinancialPlan(financialPlanId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, financialPlanId);
		}

		public ActionResult OnGetCancel()
		{
			try
			{
				long caseUrn;
				(caseUrn, _) = GetRouteData();

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long financialPlanId)
		{
			try
			{
				if (caseUrn == 0 || financialPlanId == 0)
					throw new Exception("Case urn or financial plan id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var financialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				DateOpened = DateTimeHelper.ParseToDisplayDate(financialPlanModel.CreatedAt);
				CaseModel.PreviousUrl = url;

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
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