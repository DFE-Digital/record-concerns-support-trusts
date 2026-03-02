using Ardalis.GuardClauses;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
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
	public List<ActiveCaseSummaryModel> AllCases { get; private set; } = [];

	public PaginationModel Pagination { get; set; } = new();

	[BindProperty]
	public CaseFilters Filters { get; set; } = new();

	public async Task<ActionResult> OnGetAsync()
	{
		_logger.LogInformation("AllCasesPageModel::OnGetAsync executed");

		try
		{
			Filters.PopulateFrom(Request.Query);

			// Get filtered cases
			var activeCaseGroup = await _caseSummaryService.GetCaseSummariesByFilter(Filters.SelectedRegionEnums, Filters.SelectedStatusEnums, PageNumber);
			AllCases = activeCaseGroup.Cases;

			Pagination = activeCaseGroup.Pagination;
			Pagination.Url = BuildBaseUrl("/All", Request.Query);
		}
		catch (Exception)
		{
			TempData["Error.Message"] = ErrorOnGetPage;
		}

		return Page();
	}

	public static string BuildBaseUrl(string path, IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
	{
		var pairs = requestQuery
			.Where(kvp =>
				!kvp.Key.Equals(nameof(PageNumber), StringComparison.OrdinalIgnoreCase) &&
				!kvp.Key.Equals("pageNumber", StringComparison.OrdinalIgnoreCase))
			.SelectMany(kvp => kvp.Value, (kvp, v) => new KeyValuePair<string, string>(kvp.Key, v))
			.ToList();

		return QueryHelpers.AddQueryString(path, pairs);
	}
}
