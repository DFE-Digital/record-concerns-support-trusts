using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action
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
				_logger.LogInformation("Case::Action::IndexPageModel::OnPostAsync");

				// Fetch case urn
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				// Form
				var action = Request.Form["action"].ToString();

				if (string.IsNullOrEmpty(action))
					throw new Exception("Missing form values");


				return RedirectToPage($"{action}/add", new { urn = caseUrn });
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::IndexPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
		}
	}
}