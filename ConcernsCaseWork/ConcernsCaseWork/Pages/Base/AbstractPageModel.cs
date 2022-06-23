using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages.Base
{
	public class AbstractPageModel : PageModel
	{
		internal const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		internal const string ErrorOnPostPage = "An error occurred posting the form, please try again. If the error persists contact the service administrator.";

		protected long GetRouteValueInt64(string routeKey)
		{
			long value = default(long);

			var rawVal = RouteData.Values[routeKey];
			if (rawVal != null)
			{
				long.TryParse(rawVal.ToString(), out value);
			}

			return value;
		}

	}

}