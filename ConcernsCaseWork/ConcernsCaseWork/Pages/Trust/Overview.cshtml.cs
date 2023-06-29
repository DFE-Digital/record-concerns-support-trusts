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
		private const int _pageCount = 5;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public PagedCaseSummaryModel ActiveCases { get; private set; }
		public PagedCaseSummaryModel ClosedCases { get; private set; }
		
		
		
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

				var trustUkprnValue = RouteData.Values["id"].ToString();
				if (string.IsNullOrEmpty(trustUkprnValue))
				{
					throw new Exception("OverviewPageModel::TrustUkrn is null or invalid to parse");
				}
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkprnValue);
				ActiveCases = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkprnValue,1,_pageCount);
				ActiveCases.PageCount = (int)Math.Ceiling((double)ActiveCases.RecordCount / _pageCount);
				ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkprnValue,1,_pageCount);
				ClosedCases.PageCount = (int)Math.Ceiling((double)ClosedCases.RecordCount / _pageCount);
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::OverviewPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task OnPostAsync(int pageNo, string caseType)
		{
			try
			{
				_logger.LogInformation("Trust::OverviewPageModel::OnPostAsync");
				if (pageNo <= 0) { pageNo = 1;}
				var trustUkprnValue = RouteData.Values["id"].ToString();
				if (string.IsNullOrEmpty(trustUkprnValue))
				{
					throw new Exception("OverviewPageModel::TrustUkrn is null or invalid to parse");
				}

				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkprnValue);
				if (caseType == "active")
				{
					ActiveCases = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkprnValue, pageNo, _pageCount);
				}
				else
				{
					ActiveCases = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkprnValue, 1, _pageCount);
				}
				ActiveCases.PageCount = ActiveCases.RecordCount / _pageCount;
				if (caseType == "closed")
				{
					ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkprnValue, pageNo, _pageCount);
				}
				else
				{
					ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkprnValue, 1, _pageCount);
				}
				ClosedCases.PageCount = ActiveCases.RecordCount / _pageCount;
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::OverviewPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}