using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
		private readonly ILogger<ManagementPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<TrustCasesModel> TrustCasesModel { get; private set; }
		public IList<CaseHistoryModel> CasesHistoryModel { get; private set; }

		public ManagementPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ICaseHistoryModelService caseHistoryModelService,
			ILogger<ManagementPageModel> logger)
		{
			_caseHistoryModelService = caseHistoryModelService;
			_trustModelService = trustModelService;
			_caseModelService = caseModelService;
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
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ManagementPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}