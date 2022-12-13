using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
	public class CookiesPageModel : AbstractPageModel
	{
		private ILogger<CookiesPageModel> _logger;

		[BindProperty]
		public bool HasConsented { get; set; }

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

		public IActionResult OnPost()
		{
			_logger.LogMethodEntered();

			var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };
			Response.Cookies.Append(CookieConstants.CookieConsentName, HasConsented.ToString(), cookieOptions);

			return Page();
		}
	}
}
