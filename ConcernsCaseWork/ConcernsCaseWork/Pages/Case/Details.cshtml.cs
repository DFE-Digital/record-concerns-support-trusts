using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace ConcernsCaseWork.Pages.Case
{
	public class DetailsModel : PageModel
	{
		public void OnGet(string selectedTrust)
		{
			Console.WriteLine($@"DetailsPAGE::OnGet::SelectedTrust - {selectedTrust}");
		}
	}
}