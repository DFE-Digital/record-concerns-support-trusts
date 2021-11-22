using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
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
		private readonly ITypeModelService _typeModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<ManagementPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public IDictionary<long, TypeModel> TypeModelMap { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<TrustCasesModel> TrustCasesModel { get; private set; }
		public IList<CaseHistoryModel> CasesHistoryModel { get; private set; }
		public IDictionary<long, RatingModel> RatingModelMap { get; private set; }

		public ManagementPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ICaseHistoryModelService caseHistoryModelService,
			ITypeModelService typeModelService,
			IRecordModelService recordModelService,
			IRatingModelService ratingModelService,
			ILogger<ManagementPageModel> logger)
		{
			_caseHistoryModelService = caseHistoryModelService;
			_trustModelService = trustModelService;
			_caseModelService = caseModelService;
			_typeModelService = typeModelService;
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
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("ManagementPageModel::CaseUrn is null or invalid to parse");
				}

				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				CasesHistoryModel = await _caseHistoryModelService.GetCasesHistory(User.Identity.Name, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				TrustCasesModel = await _caseModelService.GetCasesByTrustUkprn(CaseModel.TrustUkPrn);

				
				var recordsModel = await _recordModelService.GetRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
				RatingModelMap = recordsModel.ToDictionary(r => r.Urn, r => _ratingModelService.GetRatingByUrn(r.RatingUrn).Result);
				
				
				// TODO Integrate records model service and build a IDic<long, TypeModel>
				var typeModel = await _typeModelService.GetTypeModelByUrn(CaseModel.TypeUrn);
				TypeModelMap = new Dictionary<long, TypeModel>{ { CaseModel.RecordUrn, typeModel } };




			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ManagementPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}