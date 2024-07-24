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
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public string DateOpened { get; private set; }


		public DeletePageModel(ICaseModelService caseModelService,
			INtiUnderConsiderationModelService ntiModelService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_ntiModelService = ntiModelService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long ntiUCId = 0;
			
			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiUCId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiUCId);
		}

		public async Task<ActionResult> OnGetDeleteNTIUC()
		{
			long caseUrn = 0;
			long ntiUCId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiUCId) = GetRouteData();

				await _ntiModelService.DeleteNtiUnderConsideration(ntiUCId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiUCId);
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

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long ntiUCId)
		{
			try
			{
				if (caseUrn == 0 || ntiUCId == 0)
					throw new Exception("Case urn or ntiUCId id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var ntiUCModel = await _ntiModelService.GetNtiUnderConsideration(ntiUCId);
				DateOpened = DateTimeHelper.ParseToDisplayDate(ntiUCModel.CreatedAt);
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

		private (long caseUrn, long ntiUCId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiUCIdValue = RouteData.Values["ntiUCId"];
			if (ntiUCIdValue == null || !long.TryParse(ntiUCIdValue.ToString(), out long ntiUCId) || ntiUCId == 0)
				throw new Exception("ntiUCId is null or invalid to parse");

			return (caseUrn, ntiUCId);
		}
	}
}