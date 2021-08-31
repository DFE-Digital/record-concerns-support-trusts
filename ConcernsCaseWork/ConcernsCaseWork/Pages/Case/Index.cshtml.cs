using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly ITrustModelService _trustModelService;
		
		public IndexModel(ITrustModelService trustModelService, ILogger<IndexModel> logger)
		{
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetTrustsPartial(string searchQuery)
		{
			try
			{
				_logger.LogInformation("IndexModel::OnGetTrustsPartial");
			
				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < 2)
				{
					return Partial("_TrustsSearchResult", Array.Empty<TrustSummaryModel>());
				}

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);
				
				return Partial("_TrustsSearchResult", trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::IndexModel::OnGetTrustsPartial::Exception - {ex.Message}");
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public ActionResult OnGetSelectedTrust(string selectedTrust)
		{
			try
			{
				_logger.LogInformation("IndexModel::OnGetSelectedTrust");
				
				return RedirectToPage("", "", new { selectedTrust });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::IndexModel::OnGetSelectedTrust::Exception - {ex.Message}");
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}