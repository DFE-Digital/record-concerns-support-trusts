using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace ConcernsCaseWork.Pages.Shared
{
	public class BackPageModel : PageModel
	{
		public static bool IsExtraWidthContainer;
		
		/// <summary>
		/// Don't render component for this paths
		/// </summary>
		private const string Pattern = ".*\\+/([.\\w+]+)|management(?!.*closure)|home|login|logout|concern|overview|admin|case/rating";

		public static bool CanRender(string requestPath)
		{
			IsExtraWidthContainer = !string.IsNullOrEmpty(requestPath) && requestPath.Contains("case/closed");
			return !string.IsNullOrEmpty(requestPath) 
			       && !requestPath.Equals("/") 
			       && Regex.Matches(requestPath, Pattern, RegexOptions.IgnoreCase).Count == 0;
		}
	}
}