using ConcernsCaseWork.Pages.Base;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	public class IndexPageModel : AbstractPageModel
	{
		[ItemCanBeNull]
		public async Task<IActionResult> OnGetAsync()
		{
			return Page();
		}
	}
}
