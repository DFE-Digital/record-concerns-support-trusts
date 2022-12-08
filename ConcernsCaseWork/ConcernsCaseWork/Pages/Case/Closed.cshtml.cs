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

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosedPageModel : AbstractPageModel
	{
		private readonly ICaseSummaryService _caseSummaryService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ILogger<ClosedPageModel> _logger;
		
		public List<ClosedCaseSummaryModel> ClosedCases { get; private set; }
		
		public ClosedPageModel(ICaseSummaryService caseSummaryService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<ClosedPageModel> logger)
		{
			_caseSummaryService = caseSummaryService;
			_claimsPrincipalHelper = claimsPrincipalHelper;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("ClosedPageModel::OnGetAsync executed");
		        
				ClosedCases = await _caseSummaryService.GetClosedCaseSummariesByCaseworker(GetUserName());
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosedPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				ClosedCases = new List<ClosedCaseSummaryModel>();
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}