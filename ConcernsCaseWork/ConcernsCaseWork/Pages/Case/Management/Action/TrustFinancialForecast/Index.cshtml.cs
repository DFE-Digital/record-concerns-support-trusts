using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	public class IndexPageModel : AbstractPageModel
	{
		//private ITrustFinancialForecastService _trustFinancialForecastService;
		private ILogger _logger;

		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");
		
		[BindProperty(Name = "CaseUrn", SupportsGet = true)]
		public int Urn { get; set; }
		
		[BindProperty]
		public string Notes { get; set; }

		public TextAreaUiComponent NotesTextArea = new("notes", nameof(Notes), "Notes (optional)")
		{
			MaxLength = 2000,
		};

		public IndexPageModel(
			//ITrustFinancialForecastService trustFinancialForecastService,
			ILogger<IndexPageModel> logger)
		{
			//_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			return Page();
		}
	}
}
