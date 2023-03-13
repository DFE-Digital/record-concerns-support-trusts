using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.PageHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ConcernsCaseWork.Pages.Base
{
	[Authorize]
	public class AbstractPageModel : PageModel
	{
		public const string ErrorOnGetPage = ErrorConstants.ErrorOnGetPage;
		public const string ErrorOnPostPage = ErrorConstants.ErrorOnPostPage;
		
		// Configured in startup
		public static IPageHistoryStorageHandler PageHistoryStorageHandler { get; set; }
		
		protected bool TryGetRouteValueInt64(string routeKey, out long value)
		{
			value = default(long);
			var status = false;

			var rawVal = RouteData.Values[routeKey];
			if (rawVal != null)
			{
				status = long.TryParse(rawVal.ToString(), out value);
			}

			return status;
		}

		protected void SetErrorMessage(string errorMessage)
		{
			TempData[ErrorConstants.ErrorMessageKey] = errorMessage;
		}

		protected string GetFormValue(string propertyName) => Request.Form[propertyName].ToString().GetValueOrNullIfWhitespace();
		
		protected string GetRouteValue(string propertyName) => RouteData.Values[propertyName]?.ToString().GetValueOrNullIfWhitespace();

		protected string GetPreviousPage() => PageHistoryManager.GetPreviousPage(PageHistoryStorageHandler.GetPageHistory(HttpContext).ToList());
		protected Hyperlink BuildBackLinkFromHistory(string fallbackUrl, string label = "Back") => new (label, GetPreviousPage() ?? fallbackUrl);
	}
}