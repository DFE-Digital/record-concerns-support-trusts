using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomePageModel : PageModel
    {
	    private readonly ICaseModelService _caseModelService;
	    private readonly ILogger<HomePageModel> _logger;
	    
        public IList<HomeModel> CasesActive { get; private set; }
        public IList<HomeModel> CasesMonitoring { get; private set; }
        
        public HomePageModel(ICaseModelService caseModelService, ILogger<HomePageModel> logger)
        {
            _caseModelService = caseModelService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
	        _logger.LogInformation("HomePageModel::OnGetAsync executed");
	        
	        (CasesActive, CasesMonitoring) = await _caseModelService.GetCasesByCaseworker(User.Identity.Name);
        }
    }
}
