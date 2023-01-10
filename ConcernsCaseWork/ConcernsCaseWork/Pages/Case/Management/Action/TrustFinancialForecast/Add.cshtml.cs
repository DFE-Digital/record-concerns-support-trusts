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
		
		[BindProperty(Name = "Urn", SupportsGet = true)] public int Urn { get; init; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");
		
		[BindProperty] public string ForecastingToolRanAtSelected { get; set; }
		[BindProperty] public OptionalDateTimeUiComponent SFSOInitialReviewHappenedAt { get; set; } =
			new("sfso-initial-review-happened-at", nameof(SFSOInitialReviewHappenedAt), "When did SFSO initial review happen?");
		[BindProperty] public OptionalDateTimeUiComponent TrustRespondedAt { get; set; } 
			= new("trust-responded-at", nameof(TrustRespondedAt), "When did the trust respond?");
		[BindProperty] public string NotesEntered { get; set; }
		[BindProperty] public string WasTrustResponseSatisfactorySelected { get; set; }
		[BindProperty] public string SRMAOfferedAfterTFFSelected { get; set; }

		public TextAreaUiComponent NotesTextAreaComponent { get; } = new("notes", nameof(NotesEntered), "Notes (optional)")
		{
			MaxLength = 2000
		};
		
		public RadioButtonsUiComponent WasTrustResponseSatisfactoryComponent { get; } =
			new("trust-response-satisfactory", nameof(WasTrustResponseSatisfactorySelected), "Was the trust result satisfactory?")
			{
				RadioItems = new List<RadioItem>
				{
					new () { Text = "Satisfactory" },
					new () { Text = "Not satisfactory" }
				}
			};
		
		public RadioButtonsUiComponent SRMAOfferedAfterTFFComponent { get; } =
			new("srma-offered-after-tff",  nameof(SRMAOfferedAfterTFFSelected), "SRMA offered after TFF?")
			{
				RadioItems = new List<RadioItem>
				{
					new () { Text = "Yes" },
					new () { Text = "No" }
				}
			};
		
		public RadioButtonsUiComponent ForecastingToolRanAtComponent { get; } =
			new(ElementRootId: "forecasting-tool-ran-at", Name: nameof(ForecastingToolRanAtSelected), "When was the forecasting tool run?")
			{
				RadioItems = new List<RadioItem>
				{
					new () { Text = "Current year - Spring" },
					new () { Text = "Current year - Summer" },
					new () { Text = "Previous year - Spring" },
					new () { Text = "Previous year - Summer" }
				}
			};
		

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

	}

	public record CreateTrustFinancialForecastRequest
	{
		public string ForecastingToolRunAt { get; set; }
		public DateTimeOffset SFSOInitialReviewAt { get; set; }
		public DateTimeOffset TrustRespondedAt { get; set; }
		public string WasTrustResponseSatisfactory { get; set; }
		public string SRMAOfferedAfterTFF { get; set; }
		public string Notes { get; set; }
	}
}