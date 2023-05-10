using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using JetBrains.Annotations;
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
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<IndexPageModel> _logger;

		public readonly string ReasonErrorMessage = "Add reason for SRMA";
		public readonly string DateAcceptedErrorMessage = "Enter date trust accepted SRMA";
		public readonly string DateVisitErrorMessage = "Enter dates of visit";
		public readonly string DateReportErrorMessage = "Enter date report sent to trust";


		public SRMAModel SRMAModel { get; set; }
		public string DeclineCompleteButtonLabel { get; private set; }

		[BindProperty(Name = "Urn", SupportsGet = true)]
		public long CaseUrn { get; set; }

		[BindProperty(Name = "srmaId", SupportsGet = true)]
		public int SrmaId { get; set; }

		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		public IndexPageModel(ISRMAService srmaService, ILogger<IndexPageModel> logger)
		{
			_srmaModelService = srmaService;
			_logger = logger;
		}

		[ItemCanBeNull]
		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				await SetPageData(CaseUrn, SrmaId);

				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/srma/{SrmaId}/closed");
				}
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostComplete()
		{
			_logger.LogMethodEntered();

			try
			{
				await SetPageData(CaseUrn, SrmaId);

				PerformFullValidation(SRMAModel);

				if (!ModelState.IsValid)
				{
					return Page();
				}

				return Redirect("resolve/complete");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostDecline()
		{
			_logger.LogMethodEntered();

			try
			{
				await SetPageData(CaseUrn, SrmaId);

				PerformReasonValidation(SRMAModel);

				if (!ModelState.IsValid)
				{
					return Page();
				}

				return Redirect("resolve/decline");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostCancel()
		{
			_logger.LogMethodEntered();

			try
			{
				await SetPageData(CaseUrn, SrmaId);

				PerformReasonValidation(SRMAModel);

				if (!ModelState.IsValid)
				{
					return Page();
				}

				return Redirect("resolve/cancel");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		private async Task SetPageData(long caseId, long srmaId)
		{
			// TODO - get SRMA by case ID and SRMA ID
			SRMAModel = await _srmaModelService.GetSRMAViewModel(caseId, srmaId);

			if (SRMAModel == null)
			{
				throw new Exception("Could not load this SRMA");
			}
		}

		private void PerformReasonValidation(SRMAModel model)
		{
			if (model.Reason.Equals(SRMAReasonOffered.Unknown))
			{
				ModelState.AddModelError("Reason", ReasonErrorMessage);
			}
		}
	
		private void PerformFullValidation(SRMAModel model)
		{
			PerformReasonValidation(model);

			if (!model.Status.Equals(SRMAStatus.Deployed))
			{
				return;
			}

			if (!model.DateAccepted.HasValue)
			{
				ModelState.AddModelError("DateAccepted", DateAcceptedErrorMessage);
			}

			if (!model.DateVisitStart.HasValue || !model.DateVisitEnd.HasValue)
			{
				ModelState.AddModelError("DatesOfVisit", DateVisitErrorMessage);
			}

			if (!model.DateReportSentToTrust.HasValue)
			{
				ModelState.AddModelError("DateReportSentToTrust", DateReportErrorMessage);
			}
		}
	}
}