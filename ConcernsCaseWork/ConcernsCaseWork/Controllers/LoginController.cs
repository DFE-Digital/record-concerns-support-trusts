using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Controllers
{
	public class LoginController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<LoginController> _logger;

		public LoginController(IConfiguration configuration, ILogger<LoginController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}
		
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}
		
		public IActionResult Login(string returnUrl)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}
		
		public async Task<IActionResult> SubmitLogin(string username, string password, string returnUrl)
		{
			if (username != _configuration["app_username"] || password != _configuration["app_password"])
			{
				TempData["Error.Message"] = "Incorrect username and password";
				return RedirectToAction("Login", new {returnUrl});
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "Name")
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authenticationProperties = new AuthenticationProperties();
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity), authenticationProperties);

			var decodedUrl = string.Empty;
			
			if (!string.IsNullOrEmpty(returnUrl))
			{
				decodedUrl = WebUtility.UrlDecode(returnUrl);
			}

			if (Url.IsLocalUrl(decodedUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> SignOut()
		{
			await HttpContext.SignOutAsync();
			TempData["Success.Message"] = "Successfully signed out";
			return RedirectToAction("Login");
		}
	}
}