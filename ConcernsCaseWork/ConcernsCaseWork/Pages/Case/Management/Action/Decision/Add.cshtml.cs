
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Decision;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly IDecisionModelService _decisionModelService;
		private readonly ILogger<AddPageModel> _logger;

		public int NotesMaxLength => 2000;

		public AddPageModel(IDecisionModelService decisionModelService, ILogger<AddPageModel> logger)
		{
			_decisionModelService = decisionModelService;
			_logger = logger;
		}

		public long CaseUrn { get; set; }

		public async Task<IActionResult> OnGetAsync(long urn)
		{
			_logger.LogMethodEntered();

			try
			{
				CaseUrn = (CaseUrn)urn;
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(long urn)
		{
			_logger.LogMethodEntered();

			try
			{
				CaseUrn = (CaseUrn)urn;

				//var decisionModel = PopulateDecisionFromRequest();



				//var x = await _decisionModelService.CreateDecision(decisionModel);

			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		/*

	private DecisionModel PopulateDecisionFromRequest()
	{
		var crmEnquiryNumber = Request.Form["crm-enquiry-number"];
		var retrospectiveApproval = Request.Form["retrospective-approval"];
		var submissionRequired = Request.Form["submission-required"];
		var submissionRequiredLink = Request.Form["submission-required-link"];

		var dtr_day = Request.Form["dtr-day-request-received"];
		var dtr_month = Request.Form["dtr-month-request-received"];
		var dtr_year = Request.Form["dtr-year-request-received"];

		var total_amount_requested = Request.Form["total-amount-request"];
		var notes = Request.Form["case-decision-notes"].ToString();

		var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
		var dateStarted = DateTimeHelper.TryParseExact(dtString, out DateTime parsedDate) ? parsedDate : (DateTime?)null;

		if (!string.IsNullOrEmpty(notes))
		{
			if (notes.Length > NotesMaxLength)
			{
				throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
			}
		}



		var decisionModel = new DecisionModel
		{
			CaseUrn = CaseUrn,
			CRMEnquiryNumber = crmEnquiryNumber,
			RetrospectiveApproval = true,
			SubmissionRequired = true,
			SubmissionDocumentLink = submissionRequiredLink,
			DateEFSAReceivedRequest = parsedDate,
			TotalAmountRequested = 3,
			Notes = notes,
			DecisionTypes = new List<int>() { 1, 2, 3 },
			CreatedAt = DateTime.Now
		};


		return decisionModel;
	}
		*/


	}
}