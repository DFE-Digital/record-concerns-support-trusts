using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace ConcernsCaseWork.Pages.Shared
{
	public class BackPageModel : PageModel
	{
		/// <summary>
		/// Store here all the go back options
		/// </summary>
		public const string BrowserBackPage = "javascript: history.back()";
		
		/// <summary>
		/// Don't render component for this paths
		/// </summary>
		private const string Pattern = ".*\\+/([.\\w+]+)|management(?!.*closure)|details|home|login|logout";

		public static bool CanRender(string requestPath)
		{
			return !string.IsNullOrEmpty(requestPath) 
			       && !requestPath.Equals("/") 
			       && Regex.Matches(requestPath, Pattern, RegexOptions.IgnoreCase).Count == 0;
		}
	}
}