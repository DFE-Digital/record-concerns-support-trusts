using Ardalis.GuardClauses;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AllCasesModel : AbstractPageModel
	{
		private readonly ILogger<HomePageModel> _logger;
		private readonly ICaseSummaryService _caseSummaryService;
		
		public List<ActiveCaseSummaryModel> SearchResult { get; private set; }
		public PaginationModel Pagination { get; set; }

		public AllCasesModel(
			ILogger<HomePageModel> logger,
			ICaseSummaryService caseSummaryService)
		{
			_logger = Guard.Against.Null(logger);
			_caseSummaryService = Guard.Against.Null(caseSummaryService);
		}

		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogInformation("AllCasesModel::OnGetAsync executed");

			try
			{
				var allCaseGroup = await _caseSummaryService.SearchActiveCaseSummaries(PageNumber);
				allCaseGroup.Pagination.Url = this.Url.Page("AllCases");

				SearchResult = allCaseGroup.Cases;
				Pagination = allCaseGroup.Pagination;
			}
			catch
			{
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}
    }
}
