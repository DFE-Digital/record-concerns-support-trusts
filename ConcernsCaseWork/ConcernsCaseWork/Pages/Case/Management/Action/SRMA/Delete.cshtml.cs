using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ISRMAService _srmaModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public string SRMATitle { get; private set; }


		public DeletePageModel(ICaseModelService caseModelService,
			ISRMAService srmaService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_srmaModelService = srmaService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long srmaId = 0;
			
			try
			{
				_logger.LogMethodEntered();

				(caseUrn, srmaId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, srmaId);
		}

		public async Task<ActionResult> OnGetDeleteSRMA()
		{
			long caseUrn = 0;
			long srmaId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, srmaId) = GetRouteData();

				await _srmaModelService.DeleteSRMA(srmaId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, srmaId);
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

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long srmaId)
		{
			try
			{
				if (caseUrn == 0 || srmaId == 0)
					throw new Exception("Case urn or srma id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var srmaModel = await _srmaModelService.GetSRMAViewModel(caseUrn, srmaId);
				SRMATitle = srmaModel.Status.Description();
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
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

		private (long caseUrn, long srmaId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			return (caseUrn, srmaId);
		}
	}
}