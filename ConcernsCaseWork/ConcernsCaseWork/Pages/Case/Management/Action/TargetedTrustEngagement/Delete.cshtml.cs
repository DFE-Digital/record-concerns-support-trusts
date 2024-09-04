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
using ConcernsCaseWork.Service.TargetedTrustEngagement;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITargetedTrustEngagementService _targetedTrustEngagementService;

		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<DeletePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public string TargetedTrustEngagementTitle { get; set; }

		public DeletePageModel(ICaseModelService caseModelService,
			ITargetedTrustEngagementService targetedTrustEngagementService,
			ITrustModelService trustModelService, 
			ILogger<DeletePageModel> logger)
		{
			_caseModelService = caseModelService;
			_targetedTrustEngagementService = targetedTrustEngagementService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			int caseUrn = 0;
			int tagetedTrustEngagementId = 0;
			
			try
			{
				_logger.LogMethodEntered();
				(caseUrn, tagetedTrustEngagementId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, tagetedTrustEngagementId);
		}

		public async Task<ActionResult> OnGetDeleteTTE()
		{
			int caseUrn = 0;
			int tagetedTrustEngagementId = 0;

			try
			{
				_logger.LogMethodEntered();

				(caseUrn, tagetedTrustEngagementId) = GetRouteData();
				
				await _targetedTrustEngagementService.DeleteTargetedTrustEngagement(caseUrn, tagetedTrustEngagementId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, tagetedTrustEngagementId);
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

		private async Task<ActionResult> LoadPage(string url, int caseUrn, int tagetedTrustEngagementId)
		{
			try
			{
				if (caseUrn == 0 || tagetedTrustEngagementId == 0)
					throw new Exception("Case urn or tagetedTrustEngagement id cannot be 0");

				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var tteModel = await _targetedTrustEngagementService.GetTargetedTrustEngagement(caseUrn, tagetedTrustEngagementId);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				TargetedTrustEngagementTitle = tteModel.Title;
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

		private (int caseUrn, int tagetedTrustEngagementId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !int.TryParse(caseUrnValue.ToString(), out int caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var tagetedTrustEngagementIdValue = RouteData.Values["tagetedTrustEngagementId"];
			if (tagetedTrustEngagementIdValue == null || !int.TryParse(tagetedTrustEngagementIdValue.ToString(), out int tagetedTrustEngagementId) || tagetedTrustEngagementId == 0)
				throw new Exception("tagetedTrustEngagementId is null or invalid to parse");

			return (caseUrn, tagetedTrustEngagementId);
		}
	}
}