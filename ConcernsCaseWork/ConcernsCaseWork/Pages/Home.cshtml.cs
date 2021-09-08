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
    public class HomeModel : PageModel
    {
	    private readonly ICaseModelService _caseModelService;
	    private readonly ILogger<HomeModel> _logger;
	    
        public IList<HomeUiModel> CasesActive { get; private set; }
        public IList<HomeUiModel> CasesMonitoring { get; private set; }
        
        public HomeModel(ICaseModelService caseModelService, ILogger<HomeModel> logger)
        {
            _caseModelService = caseModelService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
	        _logger.LogInformation("HomeModel::OnGetAsync executed");
	        
	        (CasesActive, CasesMonitoring) = await _caseModelService.GetCasesByCaseworker(User.Identity.Name);
        }
    }
}
