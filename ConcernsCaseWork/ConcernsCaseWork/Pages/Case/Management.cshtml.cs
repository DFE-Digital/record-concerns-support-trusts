using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ManagementPageModel : AbstractPageModel
	{
		private readonly ICaseHistoryModelService _caseHistoryModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<ManagementPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<TrustCasesModel> TrustCasesModel { get; private set; }
		public IList<CaseHistoryModel> CasesHistoryModel { get; private set; }
		public IDictionary<long, RatingModel> RatingModelMap { get; set; }

		public ManagementPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ICaseHistoryModelService caseHistoryModelService,
			IRecordModelService recordModelService,
			IRatingModelService ratingModelService,
			ILogger<ManagementPageModel> logger)
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

				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("ManagementPageModel::CaseUrn is null or invalid to parse");
				}

				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				TrustCasesModel = await _caseModelService.GetCasesByTrustUkprn(CaseModel.TrustUkPrn);
				CasesHistoryModel = await _caseHistoryModelService.GetCasesHistory(User.Identity.Name, caseUrn);
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
				RatingModelMap = recordsModel.ToDictionary(r => r.Urn, r => _ratingModelService.GetRatingByUrn(r.RatingUrn).Result);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ManagementPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}