using ConcernsCaseWork.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages.Base
{
	public class AbstractPageModel : PageModel
	{
		internal const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		internal const string ErrorOnPostPage = "An error occurred posting the form, please try again. If the error persists contact the service administrator.";

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

		protected string GetFormValue(string propertyName) => Request.Form[propertyName].ToString().GetValueOrNullIfWhitespace();
		
		protected string GetRouteValue(string propertyName) => RouteData.Values[propertyName].ToString().GetValueOrNullIfWhitespace();
	}
}