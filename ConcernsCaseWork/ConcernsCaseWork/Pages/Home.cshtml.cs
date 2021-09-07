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
        public readonly Dictionary<int, string> Rags = new Dictionary<int, string>(5)
        {
	        {0, "-"}, {1, "Red"}, {2, "Red | Amber"}, {3, "Amber | Green"}, {4, "Red Plus"}
        };
        public readonly Dictionary<int, string> RagsCss = new Dictionary<int, string>(5)
        {
	        {0, ""}, {1, "ragtag__red"}, {2, "ragtag__redamber"}, {3, "ragtag__ambergreen"}, {4, "ragtag__redplus"}
        };

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
