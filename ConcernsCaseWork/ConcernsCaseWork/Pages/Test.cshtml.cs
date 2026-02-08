using ConcernsCaseWork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages
{
    public class TestModel : PageModel
    {
		[BindProperty]
		public bool? YesChecked { get; set; }

		[BindProperty]
		public string RiskToTrustRationalText { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent RiskToTrustRational { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			RiskToTrustRational = CaseComponentBuilder.BuildRiskToTrustRational(nameof(RiskToTrustRational), null);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int id)
		{
			if (YesChecked is true && string.IsNullOrWhiteSpace(RiskToTrustRationalText))
			{
				ModelState.AddModelError(nameof(RiskToTrustRationalText), "You must enter a RAG rationale commentary");
			}

			if (ModelState.IsValid)
			{

			}

			return Page();
		}

	}
}
