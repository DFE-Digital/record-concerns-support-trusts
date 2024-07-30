using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<ClosurePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public string ConcernTypeName { get; set; }

		public DeletePageModel(ICaseModelService caseModelService, 
			IRecordModelService recordModelService,
			ITrustModelService trustModelService, 
			ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long recordId = 0;
			
			try
			{
				_logger.LogInformation("Case::Concern::DeletePageModel::OnGet");
				(caseUrn, recordId) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::DeletePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordId);
		}

		public async Task<ActionResult> OnGetDeleteConcern()
		{
			long caseUrn = 0;
			long recordId = 0;

			try
			{
				_logger.LogInformation("Case::Concern::DeletePageModel::DeleteConcern");

				(caseUrn, recordId) = GetRouteData();
				
				var currentDate = DateTimeOffset.Now;

				await _recordModelService.DeleteRecord(caseUrn, recordId);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::DeletePageModel::DeleteConcern::OnGetDeleteConcern::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordId);
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
				_logger.LogError("Case::Concern::DeletePageModel::OnGetCancel::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long recordId)
		{
			try
			{
				if (caseUrn == 0 || recordId == 0)
					throw new Exception("Case urn or record id cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				var recordModel = await _recordModelService.GetRecordModelById(caseUrn, recordId);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				ConcernTypeName = recordModel.GetConcernTypeName();
				CaseModel.PreviousUrl = url;

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::DeletePageModel::LoadPage::Exception - {Message}", ex.Message);
					
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		} 

		private (long caseUrn, long recordId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var recordIdValue = RouteData.Values["recordId"];
			if (recordIdValue == null || !long.TryParse(recordIdValue.ToString(), out long recordId) || recordId == 0)
				throw new Exception("RecordId is null or invalid to parse");

			return (caseUrn, recordId);
		}
	}
}