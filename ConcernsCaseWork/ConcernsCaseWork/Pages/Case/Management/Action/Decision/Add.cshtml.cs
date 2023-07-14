﻿
using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreLinq;
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
		public OptionalDateTimeUiComponent ReceivedRequestDate { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "decisionId")]
		public long? DecisionId { get; set; }

		[BindProperty]
		public List<DecisionTypeQuestionModel> DecisionTypeQuestions { get; set; }

		public string SaveAndContinueButtonText { get; set; }

		public AddPageModel(IDecisionService decisionService, ILogger<AddPageModel> logger)
		{
			_decisionService = decisionService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				Decision = await CreateDecisionModel();

				LoadPageComponents(Decision);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					LoadPageComponents();
					return Page();
				}


				Decision.ReceivedRequestDate = ReceivedRequestDate.Date.ToDateTime() ?? new DateTime();
				Decision.SupportingNotes = Notes.Text.StringContents;
				Decision.DecisionTypes = ModelToDecisionTypeQuestion(DecisionTypeQuestions);

				if (DecisionId.HasValue)
				{
					var updateDecisionRequest = DecisionMapping.ToUpdateDecision(Decision);
					await _decisionService.PutDecision(CaseUrn, (long)DecisionId, updateDecisionRequest);

					return Redirect($"/case/{CaseUrn}/management/action/decision/{DecisionId}");
				}

				await _decisionService.PostDecision(Decision);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(CreateDecisionRequest model)
		{
			LoadPageComponents();

			ReceivedRequestDate.Date = new OptionalDateModel()
			{
				Day = model.ReceivedRequestDate?.Day.ToString("00"),
				Month = model.ReceivedRequestDate?.Month.ToString("00"),
				Year = model.ReceivedRequestDate?.Year.ToString()
			};

			Notes.Text.StringContents = model.SupportingNotes;

			SetDecisionTypeAnswers(DecisionTypeQuestions, model.DecisionTypes);
		}

		private void LoadPageComponents()
		{
			SetupPage();

			ReceivedRequestDate = BuildReceivedRequestDateComponent(ReceivedRequestDate?.Date);
			Notes = BuildNotesComponent(Notes?.Text.StringContents);

			var answers = new DecisionTypeQuestion[] { };

			if (DecisionTypeQuestions != null)
			{
				answers = ModelToDecisionTypeQuestion(DecisionTypeQuestions);
			}

			DecisionTypeQuestions = BuildDecisionTypeQuestionsComponent();
			SetDecisionTypeAnswers(DecisionTypeQuestions, answers);
		}

		private void SetupPage()
		{
			ViewData[ViewDataConstants.Title] = DecisionId.HasValue ? "Edit decision" : "Add decision";
			SaveAndContinueButtonText = DecisionId.HasValue ? "Save and return to decision" : "Save and return to case overview";
		}

		private async Task<CreateDecisionRequest> CreateDecisionModel()
		{
			var result = new CreateDecisionRequest();

			result.ConcernsCaseUrn = (int)CaseUrn;

			if (DecisionId.HasValue)
			{
				var apiDecision = await _decisionService.GetDecision(CaseUrn, (int)DecisionId);

				result = DecisionMapping.ToEditDecisionModel(apiDecision);
			}

			return result;
		}

		private DecisionTypeQuestion[] ModelToDecisionTypeQuestion(List<DecisionTypeQuestionModel> model)
		{
			var result = model.Where(q => q.Id != null).Select(q => new DecisionTypeQuestion()
			{
				Id = (DecisionType)q.Id,
			}).ToArray();

			return result;
		}

		private static List<DecisionTypeQuestionModel> BuildDecisionTypeQuestionsComponent()
		{
			var result = new List<DecisionTypeQuestionModel>()
			{
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.NoticeToImprove,
					Hint = "An NTI is an intervention tool. It's used to set out conditions that a trust must meet to act on area(s) of concern."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.Section128,
					Hint = "A section 128 direction gives the Secretary of State the power to block an individual from taking part in the management of an independent school (including academies and free schools)."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.QualifiedFloatingCharge,
					Hint = "A QFC helps us secure the repayment of funding we advance to an academy trust. This includes appointing an administrator, making sure the funding can be recovered and potentially disqualifying an unfit director."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.NonRepayableFinancialSupport,
					Hint = "Non-repayable grants are paid in exceptional circumstances to support a trust or academy financially."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.RepayableFinancialSupport,
					Hint = "Repayable funding are payments that trusts must repay in line with an agreed repayment plan, ideally within 3 years. Select this decision type for decisions related to existing repayable financial support, such as change to repayment schedule or drawdown of previously agreed funding."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.ShortTermCashAdvance,
					Hint = "A short-term cash advance or a general annual grant (GAG) advance is given to help an academy manage its cash flow. This should be repaid within the same academy financial year."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.WriteOffRecoverableFunding,
					Hint = "A write-off can be considered if a trust cannot repay financial support previously received from us."

				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.OtherFinancialSupport,
					Hint = "All other types of financial support for exceptional circumstances. This includes exceptional annual grant (EAG), general annual grant (GAG) protection, popular growth funding, restructuring support and start-up support."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.EstimatesFundingOrPupilNumberAdjustment,
					Hint = "Covers: a) agreements to move from lagged funding (based on pupil census data) to funding based on an estimate of the coming year’s pupil numbers, used when a school is growing; b) an adjustment to a trust's General Annual Grant (GAG) to funding based on estimated pupil numbers in line with actual pupil numbers, once these are confirmed (PNA). Also called an in-year adjustment or change in funding methodology."

				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.EsfaApproval,
					Hint = "Some versions of the funding agreement require trusts to seek approval from ESFA to spend or write off funds (also called transactions approval). Examples include as severance pay, compromise agreements or ex gratia payments; agreeing off-payroll arrangements for staff; entering into a finance lease or operating lease; or carrying forward large reserves. Trusts going ahead with these decisions or transactions without ESFA approval could be in breach of their funding agreement. This typically affects trusts under an NTI (Notice to Improve), where ESFA approval can be a condition of the NTI."
				},
				new DecisionTypeQuestionModel()
				{
					Id = DecisionType.FreedomOfInformationExemptions,
					Hint = "If information qualifies as an exemption to the Freedom of Information Act, we can decline to release information. Some exemptions require ministerial approval. You must contact the FOI team if you think you need to apply an exemption to your FOI response or if you have any concerns about releasing information as part of a response."
				}
			};

			for (var idx = 0; idx < result.Count; idx++)
			{
				var question = result[idx];

				question.Index = idx;
				question.Name = $"{nameof(DecisionTypeQuestions)}[{idx}].Id";

				if (question.Id == DecisionType.NonRepayableFinancialSupport || question.Id == DecisionType.RepayableFinancialSupport)
				{
					question.DrawdownFacilityAgreed = BuildDrawdownFacilityAgreedComponent(question);
					question.FrameworkCategory = BuildFrameworkCategoryComponent(question);
				}

				if (question.Id == DecisionType.ShortTermCashAdvance)
				{
					question.DrawdownFacilityAgreed = BuildDrawdownFacilityAgreedComponent(question);
				}
			}

			return result;
		}

		private void SetDecisionTypeAnswers(List<DecisionTypeQuestionModel> model, DecisionTypeQuestion[] answers)
		{
			answers.ForEach(answer =>
			{
				var question = model.SingleOrDefault(m => m.Id == answer.Id);

				if (question == null)
				{
					return;
				}

				question.IsChecked = true;
			});
		}

		private static OptionalDateTimeUiComponent BuildReceivedRequestDateComponent(OptionalDateModel? date = null)
		{
			return new OptionalDateTimeUiComponent("request-received", nameof(ReceivedRequestDate), "When did ESFA (Education and Skills Funding Agency) receive the request?")
			{
				Date = date,
				DisplayName = "Date ESFA received request"
			};
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("case-decision-notes", nameof(Notes), "Supporting notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action",
			Text = new ValidateableString()
			{
				MaxLength = DecisionConstants.MaxSupportingNotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};

		private static RadioButtonsUiComponent BuildDrawdownFacilityAgreedComponent(DecisionTypeQuestionModel model)
		{
			var id = $"draw-down-facility-agreed-{model.Id}";

			var result = new RadioButtonsUiComponent(id, $"{nameof(DecisionTypeQuestions)}[{model.Index}].DrawdownFacilityAgreed", "Has drawdown facility been agreed?");

			var values = new List<DrawdownFacilityAgreed>() 
			{ 
				DrawdownFacilityAgreed.Yes, 
				DrawdownFacilityAgreed.No, 
				DrawdownFacilityAgreed.PaymentUnderExistingArrangement 
			};

			result.RadioItems = values.Select(value =>
			{
				return new SimpleRadioItem(value.Description(), (int)value)
				{
					TestId = value.ToString()
				};
			});

			return result;
		}

		private static RadioButtonsUiComponent BuildFrameworkCategoryComponent(DecisionTypeQuestionModel model)
		{
			var id = $"framwork-category-{model.Id}";

			var result = new RadioButtonsUiComponent(id, $"{nameof(DecisionTypeQuestions)}[{model.Index}].FrameworkCategory", "What is the framework category?");

			var values = new List<FrameworkCategory>()
			{
				FrameworkCategory.EnablingFinancialRecovery,
				FrameworkCategory.BuildingFinancialCapability,
				FrameworkCategory.FacilitatingTransferFinanciallyAgreed,
				FrameworkCategory.FacilitatingTransferEducationallyTriggered
			};

			result.RadioItems = values.Select(value =>
			{
				return new SimpleRadioItem(value.Description(), (int)value)
				{
					TestId = value.ToString()
				};
			});

			return result;
		}
	}

	public class DecisionTypeQuestionModel
	{
		public string Name { get; set; }
		public DecisionType? Id { get; set; }
		public RadioButtonsUiComponent? DrawdownFacilityAgreed { get; set; }
		public RadioButtonsUiComponent? FrameworkCategory { get; set; }
		public string Hint { get; set; }
		public bool IsChecked { get; set; }
		/// <summary>
		/// In order to post a list of complex objects you need to track an index
		/// </summary>
		public int Index { get; set; }
	}

}