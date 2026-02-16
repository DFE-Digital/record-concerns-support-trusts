using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.All;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class AllCasesPageModel : AbstractPageModel
{
	private readonly ICaseSummaryService _caseSummaryService;
	private readonly ILogger<AllCasesPageModel> _logger;
	public List<ActiveCaseSummaryModel> AllCases { get; private set; }

	public PaginationModel Pagination { get; set; }

	[BindProperty]
	public CaseFilters Filters { get; set; } = new();

	public AllCasesPageModel(
		ILogger<AllCasesPageModel> logger,
		ICaseSummaryService caseSummaryService)
	{
		_logger = Guard.Against.Null(logger);
		_caseSummaryService = Guard.Against.Null(caseSummaryService);
	}

	public async Task<ActionResult> OnGetAsync()
	{
		_logger.LogInformation("AllCasesPageModel::OnGetAsync executed");

		try
		{
			// Initialize and persist filters using TempData
			Filters.PopulateFrom(Request.Query);

			// Get filtered cases
			var activeCaseGroup = await _caseSummaryService.GetCaseSummariesByFilter(Filters.SelectedRegionEnums, PageNumber);
			AllCases = activeCaseGroup.Cases;

			Pagination = activeCaseGroup.Pagination;
		}
		catch (Exception)
		{
			TempData["Error.Message"] = ErrorOnGetPage;
		}

		return Page();
	}

}

