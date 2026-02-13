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
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private readonly ICaseSummaryService _caseSummaryService;
	private readonly ILogger<AllCasesPageModel> _logger;
	public List<ActiveCaseSummaryModel> AllCases { get; private set; }

	public PaginationModel Pagination { get; set; }

	public AllCasesPageModel(
		ILogger<AllCasesPageModel> logger,
		ICaseSummaryService caseSummaryService,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		_caseSummaryService = Guard.Against.Null(caseSummaryService);
	}

	public async Task<ActionResult> OnGetAsync()
	{
		_logger.LogInformation("AllCasesPageModel::OnGetAsync executed");

		try
		{
			var activeCaseGroup = await _caseSummaryService.GetCaseSummariesByFilter(PageNumber);
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

