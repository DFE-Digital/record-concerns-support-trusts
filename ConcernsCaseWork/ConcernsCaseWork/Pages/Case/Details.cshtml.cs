using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace ConcernsCaseWork.Pages.Case
{
	public class DetailsModel : PageModel
	{
		public IActionResult OnGet()
		{
			var caseStateDate = TempData.Get<CaseStateData>("CaseStateData");
			
			Console.WriteLine($@"DetailsPAGE::OnGet::SelectedTrust - { caseStateDate.TrustUkPrn }");

			return Page();
		}
	}
}