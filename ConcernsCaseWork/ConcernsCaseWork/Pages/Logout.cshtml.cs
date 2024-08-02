using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	public class LogoutModel : AbstractPageModel
	{
		public async Task<ActionResult> OnGetAsync()
        {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
			{
				RedirectUri = "/"
			});
			return Page();
		}
    }
}
