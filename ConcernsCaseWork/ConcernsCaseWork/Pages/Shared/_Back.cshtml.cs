using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Shared
{
	public class BackModel : PageModel
	{
		/// <summary>
		/// Store here all the go back options
		/// </summary>
		public const string BrowserBackPage = "javascript: history.back()";
		
		/// <summary>
		/// Don't render component for this paths
		/// </summary>
		private static readonly IList<string> PathExclusions = new List<string> { "home", "login", "logout", "details" };

		public static bool CanRender(string requestPath)
		{
			return !string.IsNullOrEmpty(requestPath) && !requestPath.Equals("/") && PathExclusions.All(exclude => !requestPath.Contains(exclude));
		}
	}
}