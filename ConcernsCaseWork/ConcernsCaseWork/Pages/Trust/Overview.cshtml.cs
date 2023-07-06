using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Trust
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class OverviewPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly ILogger<OverviewPageModel> _logger;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<ClosedCaseSummaryModel> ClosedCases { get; private set; }

		public CaseSummaryGroupModel<ActiveCaseSummaryModel> ActiveCaseSummaryGroupModel { get; set; }

		[BindProperty(SupportsGet = true, Name = "id")]
		public string TrustUkPrn { get; set; }

		public OverviewPageModel(ITrustModelService trustModelService,
			ICaseSummaryService caseSummaryService,
			ITypeModelService typeModelService,
			ILogger<OverviewPageModel> logger)
		{
			_trustModelService = trustModelService;
			_caseSummaryService = caseSummaryService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Trust::OverviewPageModel::OnGetAsync");

				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(TrustUkPrn);

				ActiveCaseSummaryGroupModel = await GetActiveCases(1);

				ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByTrust(TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::OverviewPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnGetPaginatedActiveCases(int pageNumber)
		{
			var activeCaseSummaryGroup = await GetActiveCases(pageNumber);

			return Partial("_TrustActiveCases", activeCaseSummaryGroup);
		}

		private async Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCases(int pageNumber)
		{
			var result = await _caseSummaryService.GetActiveCaseSummariesByTrust(TrustUkPrn, pageNumber);
			result.Pagination.Url = $"/trust/{TrustUkPrn}/overview?handler=PaginatedActiveCases";
			result.Pagination.ContentContainerId = "active-cases";
			result.Pagination.ElementIdPrefix = "active-cases";

			return result;
		}
	}
}