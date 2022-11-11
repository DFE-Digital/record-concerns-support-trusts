
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly IDecisionService _decisionService;
		private readonly ILogger<AddPageModel> _logger;

		[BindProperty]
		public CreateDecisionRequest Decision { get; set; }

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

		public long? DecisionId { get; set; }

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public long CaseUrn { get; set; }

		public async Task<IActionResult> OnGetAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				CaseUrn = (CaseUrn)urn;
				DecisionId = decisionId;

				Decision = new();
				Decision.ConcernsCaseUrn = (int)urn;

				if (DecisionId.HasValue)
				{
					await PopulateDecisionModel(urn, (int)decisionId);
				}

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData[ErrorConstants.ErrorMessageKey] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				CaseUrn = (CaseUrn)urn;
				DecisionId = decisionId;

				if (!ModelState.IsValid)
				{
					TempData["Decision.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return Page();
				}

				PopulateCreateDecisionDtoFromRequest();

				if (decisionId.HasValue)
				{
					var updateDecisionRequest = DecisionMapping.ToUpdateDecision(Decision);
					await _decisionService.PutDecision(urn, (long)decisionId, updateDecisionRequest);

					return Redirect($"/case/{urn}/management/action/decision/{decisionId}");
				}

				await _decisionService.PostDecision(Decision);

				return Redirect($"/case/{urn}/management");
			}
			catch (InvalidUserInputException ex)
			{
				TempData["Decision.Message"] = new List<string>() { ex.Message };
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
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

			Decision.DecisionTypes = DecisionTypePropertiesToDecisionTypeArray();
			Decision.ReceivedRequestDate = parsedDate;
		}

		private async Task PopulateDecisionModel(long caseUrn, int decisionId)
		{
			var apiDecision = await _decisionService.GetDecision(caseUrn, decisionId);

			Decision = DecisionMapping.ToEditDecisionModel(apiDecision);
			DecisionTypeArrayToDecisionTypeProperties(Decision.DecisionTypes);
		}

		private void DecisionTypeArrayToDecisionTypeProperties(DecisionType[] decisionTypes)
		{
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