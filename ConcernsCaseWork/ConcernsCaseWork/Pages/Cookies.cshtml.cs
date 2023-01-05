using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class CookiesPageModel : AbstractPageModel
	{
		private ILogger<CookiesPageModel> _logger;

		[BindProperty]
		public bool HasConsented { get; set; }
		
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

		public CookiesPageModel(ILogger<CookiesPageModel> logger)
		{
			_logger = logger;
		}

		public IActionResult OnGet()
		{
			_logger.LogMethodEntered();

			bool.TryParse(Request.Cookies[CookieConstants.CookieConsentName], out bool existingValue);
			HasConsented = existingValue;

			return Page();
		}

		public IActionResult OnPost(string? bannerConsent = null)
		{
			_logger.LogMethodEntered();

			var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };

			if (bannerConsent != null)
			{
				Response.Cookies.Append(CookieConstants.CookieConsentName, bannerConsent, cookieOptions);

				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}

			Response.Cookies.Append(CookieConstants.CookieConsentName, HasConsented.ToString(), cookieOptions);

			return Page();
		}
	}
}
