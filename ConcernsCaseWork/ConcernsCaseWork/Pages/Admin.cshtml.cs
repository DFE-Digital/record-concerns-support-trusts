using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
    public class AdminPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
