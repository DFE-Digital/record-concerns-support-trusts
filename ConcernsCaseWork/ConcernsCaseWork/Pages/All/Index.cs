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
public class AllCasesPageModel(
	ILogger<AllCasesPageModel> logger,
	ICaseSummaryService caseSummaryService) : AbstractPageModel
{
	private readonly ICaseSummaryService _caseSummaryService = Guard.Against.Null(caseSummaryService);
	private readonly ILogger<AllCasesPageModel> _logger = Guard.Against.Null(logger);
	public List<ActiveCaseSummaryModel> AllCases { get; private set; }

	public PaginationModel Pagination { get; set; }

	[BindProperty]
	public CaseFilters Filters { get; set; } = new();

	public async Task<ActionResult> OnGetAsync()
	{
		_logger.LogInformation("AllCasesPageModel::OnGetAsync executed");

		try
		{
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

