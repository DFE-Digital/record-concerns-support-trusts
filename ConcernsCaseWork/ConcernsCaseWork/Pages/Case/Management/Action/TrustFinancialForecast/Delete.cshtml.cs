using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Helpers;


namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public string DateOpened { get; set; }


		public DeletePageModel(ICaseModelService caseModelService,
			ITrustFinancialForecastService trustFinancialForecastService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_trustFinancialForecastService = trustFinancialForecastService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long trustFinancialForecastId = 0;
			
			try
			{
				_logger.LogMethodEntered();

				(caseUrn, trustFinancialForecastId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, trustFinancialForecastId);
		}

		public async Task<ActionResult> OnGetDeleteTFF()
		{
			long caseUrn = 0;
			long trustFinancialForecastId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, trustFinancialForecastId) = GetRouteData();

				var request = new DeleteTrustFinancialForecastRequest() { CaseUrn = (int)caseUrn, TrustFinancialForecastId = (int)trustFinancialForecastId };

				await _trustFinancialForecastService.Delete(request);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, trustFinancialForecastId);
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

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long trustFinancialForecastId)
		{
			try
			{
				if (caseUrn == 0 || trustFinancialForecastId == 0)
					throw new Exception("Case urn or tff id cannot be 0");

				var request = new GetTrustFinancialForecastByIdRequest() { CaseUrn = (int)caseUrn,  TrustFinancialForecastId = (int)trustFinancialForecastId };

				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var tffModel = await _trustFinancialForecastService.GetById(request);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				DateOpened = DateTimeHelper.ParseToDisplayDate(tffModel.CreatedAt);
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

		private (long caseUrn, long trustFinancialForecastId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var trustFinancialForecastIdValue = RouteData.Values["trustFinancialForecastId"];
			if (trustFinancialForecastIdValue == null || !long.TryParse(trustFinancialForecastIdValue.ToString(), out long trustFinancialForecastId) || trustFinancialForecastId == 0)
				throw new Exception("trustFinancialForecastId is null or invalid to parse");

			return (caseUrn, trustFinancialForecastId);
		}
	}
}