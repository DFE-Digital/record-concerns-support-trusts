using ConcernsCaseWork.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class LoginPageModel : PageModel
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<LoginPageModel> _logger;
		private const string HomePage = "/home";

		[BindProperty]
		public CredentialModel Credentials { get; set; } = new CredentialModel{ ReturnUrl = HomePage};

		public LoginPageModel(IConfiguration configuration, ILogger<LoginPageModel> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}
		
		public IActionResult OnGet(string returnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return Redirect(Credentials.ReturnUrl);
			}
				
			Credentials.ReturnUrl = returnUrl ?? HomePage;

			return Page();
		}

		/// <summary>
		/// TODO Replace with Azure AD authentication
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public async Task<IActionResult> OnPostAsync(string returnUrl)
		{
			Credentials.ReturnUrl = returnUrl ?? HomePage;
			
			if (Credentials.Validate() || Credentials.UserName != _configuration["app:username"] || Credentials.Password != _configuration["app:password"])
			{
				_logger.LogInformation($"LoginPageModel::Invalid username or password - {Credentials.UserName}");
				
				TempData["Error.Message"] = "Incorrect username and password";
				return Page();
			}

			// TODO move code to security package
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, Credentials.UserName)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authenticationProperties = new AuthenticationProperties();
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity), authenticationProperties);

			var decodedUrl = string.Empty;
			
			if (!string.IsNullOrEmpty(Credentials.ReturnUrl))
			{
				decodedUrl = WebUtility.UrlDecode(Credentials.ReturnUrl);
			}

			return Redirect(Url.IsLocalUrl(decodedUrl) ? Credentials.ReturnUrl : HomePage);
		}
	}
}