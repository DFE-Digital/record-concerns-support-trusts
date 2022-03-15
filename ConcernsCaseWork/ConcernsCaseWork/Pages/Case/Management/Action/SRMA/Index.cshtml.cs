using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ILogger<IndexPageModel> _logger;

		public IndexPageModel(ILogger<IndexPageModel> logger)
		{
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			try
			{

			}
			catch (Exception ex)
			{

			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				
			}
			catch (Exception ex)
			{
			}

			return RedirectToPage("index");
		}
	}
}