using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class LogoutPageModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
		{
			TempData["Message.UserName"] = HttpContext.User.Identity.Name;
			await HttpContext.SignOutAsync();
			
			return Page();
		}
	}
}