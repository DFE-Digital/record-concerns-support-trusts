
using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
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
		public OptionalDateModel ReceivedRequestDate { get; set; }

		public int NotesMaxLength => DecisionConstants.MaxSupportingNotesLength;

		public long? DecisionId { get; set; }

		public long CaseUrn { get; set; }

		public List<DecisionTypeCheckBox> DecisionTypeCheckBoxes { get; set; }

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId);

				Decision = await CreateDecisionModel(urn, decisionId);

				ReceivedRequestDate = new OptionalDateModel()
				{
					Day = Decision.ReceivedRequestDate?.Day.ToString("00"),
					Month = Decision.ReceivedRequestDate?.Month.ToString("00"),
					Year = Decision.ReceivedRequestDate?.Year.ToString()
				};

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(long urn, long? decisionId = null)
		{
			_logger.LogMethodEntered();

			try
			{
				SetupPage(urn, decisionId);

				if (!ModelState.IsValid)
				{
					TempData["Decision.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return Page();
				}

				Decision.ReceivedRequestDate = ParseDate(ReceivedRequestDate);

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

		private void SetupPage(long caseUrn, long? decisionId)
		{
			ViewData[ViewDataConstants.Title] = decisionId.HasValue ? "Edit decision" : "Add decision";

			CaseUrn = (CaseUrn)caseUrn;
			DecisionId = decisionId;

			DecisionTypeCheckBoxes = BuildDecisionTypeCheckBoxes();
		}

		private async Task<CreateDecisionRequest> CreateDecisionModel(long caseUrn, long? decisionId)
		{
			var result = new CreateDecisionRequest();

			result.ConcernsCaseUrn = (int)caseUrn;

			if (decisionId.HasValue)
			{
				var apiDecision = await _decisionService.GetDecision(caseUrn, (int)decisionId);

				result = DecisionMapping.ToEditDecisionModel(apiDecision);
			}

			return result;
		}

		private DateTime? ParseDate(OptionalDateModel date)
		{
			if (date.IsEmpty())
			{
				return new DateTime();
			}

			var result = DateTimeHelper.ParseExact(date.ToString());

			return result;
		}

		private List<DecisionTypeCheckBox> BuildDecisionTypeCheckBoxes()
		{
			var result = new List<DecisionTypeCheckBox>()
			{
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.NoticeToImprove,
					Hint = "An NTI is an intervention tool. It's used to set out conditions that a trust must meet to act on area(s) of concern."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.Section128,
					Hint = "A section 128 direction gives the Secretary of State the power to block an individual from taking part in the management of an independent school (including academies and free schools)."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.QualifiedFloatingCharge,
					Hint = "A QFC helps us secure the repayment of funding we advance to an academy trust. This includes appointing an administrator, making sure the funding can be recovered and potentially disqualifying an unfit director."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.NonRepayableFinancialSupport,
					Hint = "Non-repayable grants are paid in exceptional circumstances to support a trust or academy financially."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.RepayableFinancialSupport,
					Hint = "Repayable funding are payments that trusts must repay in line with an agreed repayment plan, ideally within 3 years."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.ShortTermCashAdvance,
					Hint = "A short-term cash advance or a general annual grant (GAG) advance is given to help an academy manage its cash flow. This should be repaid within the same academy financial year."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.WriteOffRecoverableFunding,
					Hint = "A write-off can be considered if a trust cannot repay financial support previously received from us."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.OtherFinancialSupport,
					Hint = "All other types of financial support for exceptional circumstances. This includes exceptional annual grant (EAG), popular growth funding, restructuring support and start-up support."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.EstimatesFundingOrPupilNumberAdjustment,
					Hint = "Covers: a) agreements to move from lagged funding (based on pupil census data) to funding based on an estimate of the coming year’s pupil numbers, used when a school is growing; b) an adjustment to a trust's General Annual Grant (GAG) to funding based on estimated pupil numbers in line with actual pupil numbers, once these are confirmed (PNA). Also called an in-year adjustment."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.EsfaApproval,
					Hint = "Some versions of the funding agreement require trusts to seek approval from ESFA to spend or write off funds, such as severance pay or agreeing off-payroll arrangements for staff. Trusts going ahead with these decisions or transactions would be in breach of their funding agreement. Also called transactions approval. This typically affects trusts under an NTI (Notice to Improve)."
				},
				new DecisionTypeCheckBox()
				{
					DecisionType = DecisionType.FreedomOfInformationExemptions,
					Hint = "If information qualifies as an exemption to the Freedom of Information Act, we can decline to release information. Some exemptions require ministerial approval. You must contact the FOI team if you think you need to apply an exemption to your FOI response or if you have any concerns about releasing information as part of a response."
				}
			};

			return result;
		}
	}

	public class DecisionTypeCheckBox
	{
		public DecisionType DecisionType { get; set; }
		public string Hint { get; set; }
	}

}