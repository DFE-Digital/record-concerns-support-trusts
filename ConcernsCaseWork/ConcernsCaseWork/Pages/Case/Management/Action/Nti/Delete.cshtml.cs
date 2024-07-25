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
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Services.Nti;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public string DateOpened { get; private set; }


		public DeletePageModel(ICaseModelService caseModelService,
			INtiModelService ntiModelService,
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
			long ntiId = 0;
			
			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiId);
		}

		public async Task<ActionResult> OnGetDeleteNTI()
		{
			long caseUrn = 0;
			long ntiId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiId) = GetRouteData();

				await _ntiModelService.DeleteNtiByIdAsync(caseUrn, ntiId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiId);
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

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long ntiId)
		{
			try
			{
				if (caseUrn == 0 || ntiId == 0)
					throw new Exception("Case urn or ntiId id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var ntiModel = await _ntiModelService.GetNtiByIdAsync(ntiId);
				DateOpened = DateTimeHelper.ParseToDisplayDate(ntiModel.CreatedAt);
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

		private (long caseUrn, long ntiId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiIdValue = RouteData.Values["ntiId"];
			if (ntiIdValue == null || !long.TryParse(ntiIdValue.ToString(), out long ntiId) || ntiId == 0)
				throw new Exception("ntiId is null or invalid to parse");

			return (caseUrn, ntiId);
		}
	}
}