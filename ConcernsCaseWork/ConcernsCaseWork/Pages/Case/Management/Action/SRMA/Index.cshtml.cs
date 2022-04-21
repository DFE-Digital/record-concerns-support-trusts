using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Enums;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<IndexPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }
		public string DeclineCompleteButtonLabel { get; private set; }

		public IndexPageModel(ISRMAService srmaService, ILogger<IndexPageModel> logger)
		{
			_srmaModelService = srmaService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;

			try
			{
				_logger.LogInformation("Case::Action::SRMA::IndexPageModel::OnGetAsync");

				(caseUrn, srmaId) = GetRouteData();
				// TODO - get SRMA by case ID and SRMA ID
				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				if (SRMAModel == null)
				{
					throw new InvalidOperationException("Could not load this SRMA");
				}

				switch (SRMAModel.Status)
				{
					case SRMAStatus.Deployed:
						DeclineCompleteButtonLabel = "SRMA complete";
						break;
					default:
						DeclineCompleteButtonLabel = "SRMA declined";
						break;
				}
			}
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			return RedirectToPage("index");
		}

		public async Task<ActionResult> OnGetCancel()
		{
			try
			{
				return Redirect("resolve");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::IndexPageModel::OnGetCancel::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private (long caseUrn, long srmaId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			return (caseUrn, srmaId);
		}
	}
}