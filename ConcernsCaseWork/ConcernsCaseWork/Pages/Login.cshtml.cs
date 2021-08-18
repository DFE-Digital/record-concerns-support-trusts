using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class LoginModel : PageModel
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<LoginModel> _logger;
		private const string HomePage = "/home";

		[BindProperty]
		public Credential Credentials { get; set; } = new Credential{ ReturnUrl = HomePage};

		public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
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
				
			Credentials.ReturnUrl = returnUrl;

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
			
			if (Credentials.UserName != _configuration["app_username"] || Credentials.Password != _configuration["app_password"])
			{
				_logger.LogInformation($"LoginModel::Invalid username or password - {Credentials.UserName}");
				
				TempData["Error.Message"] = "Incorrect username and password";
				return Page();
			}

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
		
		public class Credential
		{
			[Required]
			public string UserName { get; set; }
			
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }
			
			public string ReturnUrl { get; set; }
		}
	}
}