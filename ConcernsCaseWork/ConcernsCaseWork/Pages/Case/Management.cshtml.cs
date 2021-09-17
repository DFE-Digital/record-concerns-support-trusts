using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
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
	public class ManagementPageModel : PageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		
		private const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		
		[BindProperty(SupportsGet = true)]
		public long Id { get; set; }
		
		public CaseModel CaseModel { get; private set; }
		
		public ManagementPageModel(ICaseModelService caseModelService, ILogger<DetailsPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ManagementPageModel::OnGetAsync");
				
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, Id);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ManagementPageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}