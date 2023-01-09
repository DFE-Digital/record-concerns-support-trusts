using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
// using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.TrustFinancialForecasts;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
// using ConcernsCaseWork.Service.TrustFinancialForecast;
// using ConcernsCaseWork.Services.TrustFinancialForecasts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		//private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<AddPageModel> _logger;
		
		[BindProperty] public string ForecastingToolRanAtSelected { get; set; }
		[BindProperty] public OptionalDateTimeUiComponent SFSOInitialReviewHappenedAt { get; set; } = new(ElementRootId: "sfso-initial-review-happened-at", Name: nameof(SFSOInitialReviewHappenedAt));
		[BindProperty] public OptionalDateTimeUiComponent TrustRespondedAt { get; set; } = new(ElementRootId: "trust-responded-at", Name: nameof(TrustRespondedAt));
		[BindProperty(Name = "Urn", SupportsGet = true)] public int Urn { get; set; }
		[BindProperty] public string Notes { get; set; }
		[BindProperty] public string WasTrustResponseSatisfactorySelected { get; set; }
		[BindProperty] public string SRMAOfferedAfterTFFSelected { get; set; }

		public TextAreaUiComponent NotesTextAreaComponent { get; } = new()
		{
			ElementRootId = "notes",
			Contents = string.Empty,
			Heading = "Notes (optional)",
			MaxLength = 2000,
			Name = nameof(Notes)
		};
		
		public RadioButtonsUiComponent WasTrustResponseSatisfactoryComponent { get; } =
			new(ElementRootId: "trust-response-satisfactory", Name: nameof(WasTrustResponseSatisfactorySelected))
			{
				RadioItems = new List<RadioItem>()
				{
					new () { Text = "Satisfactory" },
					new () { Text = "Not satisfactory" }
				}
			};
		
		public RadioButtonsUiComponent SRMAOfferedAfterTFFComponent  { get; } =
			new(ElementRootId: "srma-offered-after-tff", Name: nameof(SRMAOfferedAfterTFFSelected))
			{
				RadioItems = new List<RadioItem>()
				{
					new () { Text = "Yes" },
					new () { Text = "No" }
				}
			};
		
		public RadioButtonsUiComponent ForecastingToolRanAtComponent  { get; } =
			new(ElementRootId: "forecasting-tool-ran-at", Name: nameof(ForecastingToolRanAtSelected))
			{
				RadioItems = new List<RadioItem>()
				{
					new () { Text = "Current year - Spring" },
					new () { Text = "Current year - Summer" },
					new () { Text = "Previous year - Spring" },
					new () { Text = "Previous year - Summer" }
				}
			};
		
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");

		public AddPageModel(
			//ITrustFinancialForecastService trustFinancialForecastService, 
			ILogger<AddPageModel> logger)
		{
			//_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				//SetupPage(urn, trustFinancialForecastId);

				// if (!ModelState.IsValid)
				// {
				// 	TempData["TrustFinancialForecast.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				// 	return Page();
				// }
				//
				// TrustFinancialForecast.ReceivedRequestDate = ParseDate(ReceivedRequestDate);
				//
				// if (trustFinancialForecastId.HasValue)
				// {
				// 	var updateTrustFinancialForecastRequest = TrustFinancialForecastMapping.ToUpdateTrustFinancialForecast(TrustFinancialForecast);
				// 	await _trustFinancialForecastService.PutTrustFinancialForecast(urn, (long)trustFinancialForecastId, updateTrustFinancialForecastRequest);
				//
				// 	return Redirect($"/case/{urn}/management/action/trustFinancialForecast/{trustFinancialForecastId}");
				// }
				//
				// await _trustFinancialForecastService.PostTrustFinancialForecast(TrustFinancialForecast);

				return Redirect($"/case/{Urn}/management");
			}
			catch (InvalidUserInputException ex)
			{
				TempData["TrustFinancialForecast.Message"] = new List<string>() { ex.Message };
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}

	// 	private void SetupPage(long caseUrn, long? trustFinancialForecastId)
	// 	{
	// 		ViewData[ViewDataConstants.Title] = trustFinancialForecastId.HasValue ? "Edit trustFinancialForecast" : "Add trustFinancialForecast";
	//
	// 		CaseUrn = (CaseUrn)caseUrn;
	// 		TrustFinancialForecastId = trustFinancialForecastId;
	//
	// 		TrustFinancialForecastTypeCheckBoxes = BuildTrustFinancialForecastTypeCheckBoxes();
	// 	}
	//
	// 	private async Task<CreateTrustFinancialForecastRequest> CreateTrustFinancialForecastModel(long caseUrn, long? trustFinancialForecastId)
	// 	{
	// 		var result = new CreateTrustFinancialForecastRequest();
	//
	// 		result.ConcernsCaseUrn = (int)caseUrn;
	//
	// 		if (trustFinancialForecastId.HasValue)
	// 		{
	// 			var apiTrustFinancialForecast = await _trustFinancialForecastService.GetTrustFinancialForecast(caseUrn, (int)trustFinancialForecastId);
	//
	// 			result = TrustFinancialForecastMapping.ToEditTrustFinancialForecastModel(apiTrustFinancialForecast);
	// 		}
	//
	// 		return result;
	// 	}
	//
	// 	private DateTime ParseDate(OptionalDateModel date)
	// 	{
	// 		if (date.IsEmpty())
	// 		{
	// 			return new DateTime();
	// 		}
	//
	// 		var result = DateTimeHelper.ParseExact(date.ToString());
	//
	// 		return result;
	// 	}
	//
	// 	private List<TrustFinancialForecastTypeCheckBox> BuildTrustFinancialForecastTypeCheckBoxes()
	// 	{
	// 		var result = new List<TrustFinancialForecastTypeCheckBox>()
	// 		{
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.NoticeToImprove,
	// 				Hint = "An NTI is an intervention tool. It's used to set out conditions that a trust must meet to act on area(s) of concern."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.Section128,
	// 				Hint = "A section 128 direction gives the Secretary of State the power to block an individual from taking part in the management of an independent school (including academies and free schools)."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.QualifiedFloatingCharge,
	// 				Hint = "A QFC helps us secure the repayment of funding we advance to an academy trust. This includes appointing an administrator, making sure the funding can be recovered and potentially disqualifying an unfit director."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.NonRepayableFinancialSupport,
	// 				Hint = "Non-repayable grants are paid in exceptional circumstances to support a trust or academy financially."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.RepayableFinancialSupport,
	// 				Hint = "Repayable funding are payments that trusts must repay in line with an agreed repayment plan, ideally within 3 years."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.ShortTermCashAdvance,
	// 				Hint = "A short-term cash advance or a general annual grant (GAG) advance is given to help an academy manage its cash flow. This should be repaid within the same academy financial year."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.WriteOffRecoverableFunding,
	// 				Hint = "A write-off can be considered if a trust cannot repay financial support previously received from us."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.OtherFinancialSupport,
	// 				Hint = "All other types of financial support for exceptional circumstances. This includes exceptional annual grant (EAG), popular growth funding, restructuring support and start-up support."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.EstimatesFundingOrPupilNumberAdjustment,
	// 				Hint = "Covers: a) agreements to move from lagged funding (based on pupil census data) to funding based on an estimate of the coming year’s pupil numbers, used when a school is growing; b) an adjustment to a trust's General Annual Grant (GAG) to funding based on estimated pupil numbers in line with actual pupil numbers, once these are confirmed (PNA). Also called an in-year adjustment."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.EsfaApproval,
	// 				Hint = "Some versions of the funding agreement require trusts to seek approval from ESFA to spend or write off funds, such as severance pay or agreeing off-payroll arrangements for staff. Trusts going ahead with these trustFinancialForecasts or transactions would be in breach of their funding agreement. Also called transactions approval. This typically affects trusts under an NTI (Notice to Improve)."
	// 			},
	// 			new TrustFinancialForecastTypeCheckBox()
	// 			{
	// 				TrustFinancialForecastType = TrustFinancialForecastType.FreedomOfInformationExemptions,
	// 				Hint = "If information qualifies as an exemption to the Freedom of Information Act, we can decline to release information. Some exemptions require ministerial approval. You must contact the FOI team if you think you need to apply an exemption to your FOI response or if you have any concerns about releasing information as part of a response."
	// 			}
	// 		};
	//
	// 		return result;
	// 	}
	// }
	//
	// public class TrustFinancialForecastTypeCheckBox
	// {
	// 	public TrustFinancialForecastType TrustFinancialForecastType { get; set; }
	// 	public string Hint { get; set; }
	}

	public record CreateTrustFinancialForecastRequest
	{
		public DateTimeOffset ForecastingToolRunAt { get; set; }
		public DateTimeOffset SFSOInitialReviewAt { get; set; }
		public DateTimeOffset TrustRespondedAt { get; set; }
	}
}