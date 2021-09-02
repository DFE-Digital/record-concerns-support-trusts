using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<IndexModel> _logger;

		public DetailsModel(ITrustModelService trustModelService, ILogger<IndexModel> logger)
		{
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnGetAsync");
				
				// Get temp data from case page.
				var caseStateDate = TempData.Get<CaseStateData>("CaseStateData");
				if (caseStateDate is null)
				{
					throw new Exception("TempData CaseStateData is null");
				}
				
				
				Console.WriteLine($@"DetailsPAGE::OnGet::SelectedTrust - { caseStateDate.TrustUkPrn }");
				
				
				
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsModel::OnGetAsync::Exception - { ex.Message }");
			}


			return Page();
		}
	}
}