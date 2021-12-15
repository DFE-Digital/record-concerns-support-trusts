using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ICaseHistoryModelService _caseHistoryModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<IndexPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<TrustCasesModel> TrustCasesModel { get; private set; }
		public IList<CaseHistoryModel> CasesHistoryModel { get; private set; }

		public IndexPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ICaseHistoryModelService caseHistoryModelService,
			IRecordModelService recordModelService,
			IRatingModelService ratingModelService,
			ILogger<IndexPageModel> logger)
		{
			_caseHistoryModelService = caseHistoryModelService;
			_trustModelService = trustModelService;
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ManagementPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				// Get Case
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				
				// Map Case Rating
				CaseModel.RatingModel = await _ratingModelService.GetRatingModelByUrn(CaseModel.RatingUrn);
				
				// Get Case concerns
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
				
				// Map Case concerns
				CaseModel.RecordsModel = recordsModel;

				var caseHistoryTask = _caseHistoryModelService.GetCasesHistory(User.Identity.Name, caseUrn);
				var trustDetailsTask = _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				var trustCasesTask = _caseModelService.GetCasesByTrustUkprn(CaseModel.TrustUkPrn);

				Task.WaitAll(caseHistoryTask, trustDetailsTask, trustCasesTask);

				CasesHistoryModel = caseHistoryTask.Result;
				TrustDetailsModel = trustDetailsTask.Result;
				TrustCasesModel = trustCasesTask.Result;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ManagementPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public bool UserHasEditCasePrivileges()
		{
			bool result = CaseModel.CreatedBy.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase);
			return result;
		}
	}
}