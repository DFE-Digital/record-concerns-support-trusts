
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Decision;
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
		private readonly IDecisionService _decisionService;
		private readonly ILogger<AddPageModel> _logger;

		[BindProperty]
		public CreateDecisionDto CreateDecisionDto { get; set; }

		public int NotesMaxLength => 2000;

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
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

				if (!ModelState.IsValid)
				{
					TempData["Decision.Message"] = string.Join(",\r\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray());
					return Page();
				}

				PopulateCreateDecisionDtoFromRequest();
				await _decisionService.PostDecision(CreateDecisionDto);

				return Redirect($"/case/{CaseUrn}/management");

			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		

		private void PopulateCreateDecisionDtoFromRequest()
		{
			var dtr_day = Request.Form["dtr-day-request-received"];
			var dtr_month = Request.Form["dtr-month-request-received"];
			var dtr_year = Request.Form["dtr-year-request-received"];
			var dtString = $"{dtr_day}-{dtr_month}-{dtr_year}";
			var dateStarted = DateTimeHelper.TryParseExact(dtString, out DateTime parsedDate) ? parsedDate : (DateTime?)null;

			var decisionTypesString = Request.Form["type"].ToString();
			var decisionTypes = !string.IsNullOrEmpty(decisionTypesString) ? decisionTypesString.Split(',').Select(t => { return Int32.Parse(t); }) : Array.Empty<int>();

			CreateDecisionDto.DecisionTypes = decisionTypes;
			CreateDecisionDto.ReceivedRequestDate = parsedDate;
			CreateDecisionDto.CreatedAt = DateTimeOffset.Now;
			CreateDecisionDto.UpdatedAt = DateTimeOffset.Now;
		}
	}
}