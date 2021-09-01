using ConcernsCaseWork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;

namespace ConcernsCaseWork.Pages.Case
{
	public class DetailsModel : PageModel
	{
		[TempData]
		public string CaseState { get; set; }
		
		public IActionResult OnGet()
		{
			var caseStateDate = JsonConvert.DeserializeObject<CaseStateData>(CaseState);
			
			Console.WriteLine($@"DetailsPAGE::OnGet::SelectedTrust - { caseStateDate.TrustUkPrn }");

			return Page();
		}
	}
}