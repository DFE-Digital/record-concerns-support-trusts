
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Exceptions;
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

		[BindProperty]
		public bool DecisionTypeNoticeToImprove { get; set; }

		[BindProperty]
		public bool DecisionTypeSection128 { get; set; }

		[BindProperty]
		public bool DecisionTypeQualifiedFloatingCharge { get; set; }

		[BindProperty]
		public bool DecisionTypeNonRepayableFinancialSupport { get; set; }

		[BindProperty]
		public bool DecisionTypeRepayableFinancialSupport { get; set; }

		[BindProperty]
		public bool DecisionTypeShortTermCashAdvance { get; set; }

		[BindProperty]
		public bool DecisionTypeWriteOffRecoverableFunding { get; set; }

		[BindProperty]
		public bool DecisionTypeOtherFinancialSupport { get; set; }

		[BindProperty]
		public bool DecisionTypeEstimatesFundingOrPupilNumberAdjustment { get; set; }

		[BindProperty]
		public bool DecisionTypeEsfaApproval { get; set; }

		[BindProperty]
		public bool DecisionTypeFreedomOfInformationExemptions { get; set; }

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

				// if editing, pass the current dto state, if new pass null
				DecisionTypeArrayToDecisionTypeProperties(null);
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
					TempData["Decision.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return Page();
				}

				PopulateCreateDecisionDtoFromRequest();
				await _decisionService.PostDecision(CreateDecisionDto);

				return Redirect($"/case/{CaseUrn}/management");

			}
			catch (InvalidUserInputException ex)
			{

				TempData["Decision.Message"] = new List<string>() { ex.Message };
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
			var isValidDate = DateTimeHelper.TryParseExact(dtString, out DateTime parsedDate);

			if (dtString != "--" && !isValidDate)
			{
				throw new InvalidUserInputException($"{dtString} is an invalid date");
			}

			CreateDecisionDto.DecisionTypes = DecisionTypePropertiesToDecisionTypeArray();
			CreateDecisionDto.ReceivedRequestDate = parsedDate;
			CreateDecisionDto.CreatedAt = DateTimeOffset.Now;
			CreateDecisionDto.UpdatedAt = DateTimeOffset.Now;
		}

		private void DecisionTypeArrayToDecisionTypeProperties(DecisionType[] decisionTypes)
		{	
			if (decisionTypes == null)
			{
				return;
			}

			DecisionTypeNoticeToImprove = decisionTypes.Contains(DecisionType.NoticeToImprove);
			DecisionTypeSection128 = decisionTypes.Contains(DecisionType.Section128);
			DecisionTypeQualifiedFloatingCharge = decisionTypes.Contains(DecisionType.QualifiedFloatingCharge);
			DecisionTypeNonRepayableFinancialSupport = decisionTypes.Contains(DecisionType.NonRepayableFinancialSupport);
			DecisionTypeRepayableFinancialSupport = decisionTypes.Contains(DecisionType.RepayableFinancialSupport);
			DecisionTypeShortTermCashAdvance = decisionTypes.Contains(DecisionType.ShortTermCashAdvance);
			DecisionTypeWriteOffRecoverableFunding = decisionTypes.Contains(DecisionType.WriteOffRecoverableFunding);
			DecisionTypeOtherFinancialSupport = decisionTypes.Contains(DecisionType.OtherFinancialSupport);
			DecisionTypeEstimatesFundingOrPupilNumberAdjustment = decisionTypes.Contains(DecisionType.EstimatesFundingOrPupilNumberAdjustment);
			DecisionTypeEsfaApproval = decisionTypes.Contains(DecisionType.EsfaApproval);
			DecisionTypeFreedomOfInformationExemptions = decisionTypes.Contains(DecisionType.FreedomOfInformationExemptions);
		}

		// update the dto with the result of a call to this, so createDecisionDto.DecisionTypes = ToDecisionTypesArray([page property]), 
		private DecisionType[] DecisionTypePropertiesToDecisionTypeArray()
		{			 
			return new DecisionType[]
			{
				DecisionTypeNoticeToImprove ? DecisionType.NoticeToImprove : 0,
				DecisionTypeSection128 ? DecisionType.Section128 : 0,
				DecisionTypeQualifiedFloatingCharge ? DecisionType.QualifiedFloatingCharge : 0,
				DecisionTypeNonRepayableFinancialSupport ? DecisionType.NonRepayableFinancialSupport : 0,
				DecisionTypeRepayableFinancialSupport ? DecisionType.RepayableFinancialSupport : 0,
				DecisionTypeShortTermCashAdvance ? DecisionType.ShortTermCashAdvance : 0,
				DecisionTypeWriteOffRecoverableFunding ? DecisionType.WriteOffRecoverableFunding : 0,
				DecisionTypeOtherFinancialSupport ? DecisionType.OtherFinancialSupport : 0,
				DecisionTypeEstimatesFundingOrPupilNumberAdjustment ? DecisionType.EstimatesFundingOrPupilNumberAdjustment : 0,
				DecisionTypeEsfaApproval ? DecisionType.EsfaApproval : 0,
				DecisionTypeFreedomOfInformationExemptions ? DecisionType.FreedomOfInformationExemptions : 0,
			}.Where(x => x > 0).ToArray();
		}
	}
}