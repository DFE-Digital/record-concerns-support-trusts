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
using ConcernsCaseWork.Services.NtiWarningLetter;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public string DateOpened { get; private set; }


		public DeletePageModel(ICaseModelService caseModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long ntiWLId = 0;
			
			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiWLId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiWLId);
		}

		public async Task<ActionResult> OnGetDeleteNTIWL()
		{
			long caseUrn = 0;
			long ntiWLId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, ntiWLId) = GetRouteData();

				await _ntiWarningLetterModelService.DeleteNtiWarningLetter(caseUrn, ntiWLId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, ntiWLId);
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

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long ntiWLId)
		{
			try
			{
				if (caseUrn == 0 || ntiWLId == 0)
					throw new Exception("Case urn or ntiWLId id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var ntiWLModel = await _ntiWarningLetterModelService.GetNtiWarningLetterId(ntiWLId);
				DateOpened = DateTimeHelper.ParseToDisplayDate(ntiWLModel.CreatedAt);
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

		private (long caseUrn, long ntiWLId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiWLIdValue = RouteData.Values["ntiWLId"];
			if (ntiWLIdValue == null || !long.TryParse(ntiWLIdValue.ToString(), out long ntiWLId) || ntiWLId == 0)
				throw new Exception("ntiUCId is null or invalid to parse");

			return (caseUrn, ntiWLId);
		}
	}
}