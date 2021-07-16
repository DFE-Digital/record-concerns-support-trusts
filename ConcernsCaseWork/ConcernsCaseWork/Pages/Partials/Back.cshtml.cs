using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages.Partials
{
	public class BackModel : PageModel
	{
		/// <summary>
		/// Store here all the go back options
		/// </summary>
		public const string BrowserBackPage = "javascript: history.back()";
	}
}