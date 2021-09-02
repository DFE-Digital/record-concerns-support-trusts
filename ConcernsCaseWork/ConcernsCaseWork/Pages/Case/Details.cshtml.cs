using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICasesCachedService _casesCachedService;
		private readonly ILogger<IndexModel> _logger;

		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public DetailsModel(ITrustModelService trustModelService, ICasesCachedService casesCachedService, ILogger<IndexModel> logger)
		{
			_trustModelService = trustModelService;
			_casesCachedService = casesCachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsModel::OnGetAsync");
				
				// Get temp data from case page.
				var caseStateData = await _casesCachedService.GetCaseData<CaseStateData>(User.Identity.Name);
				if (caseStateData is null)
				{
					throw new Exception("DetailsModel::OnGetAsync::Cache CaseStateData is null");
				}

				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseStateData.TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = "Something went wrong loading the page, please try again.";
			}
		}
	}
}