using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages.Base
{
	public class AbstractPageModel : PageModel
	{
		public const string ErrorOnGetPage = ErrorConstants.ErrorOnGetPage;
		public const string ErrorOnPostPage = ErrorConstants.ErrorOnPostPage;

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
	}
}