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
	[BindProperties]
	public class AddPageModel : AbstractPageModel
	{
		//private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<AddPageModel> _logger;
		
		[BindProperty(SupportsGet = true)] public int Urn { get; init; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");
		public OptionalDateTimeUiComponent SFSOInitialReviewHappenedAt { get; set; } = new("sfso-initial-review-happened-at", nameof(SFSOInitialReviewHappenedAt), "When did SFSO initial review happen?");
		public OptionalDateTimeUiComponent TrustRespondedAt { get; set; } = new("trust-responded-at", nameof(TrustRespondedAt), "When did the trust respond?");

		public TextAreaUiComponent Notes { get; set; } 
			= new("notes", nameof(Notes), "Notes (optional)")
			{
				MaxLength = 2000
			};
		
		public RadioButtonsUiComponent WasTrustResponseSatisfactory { get; set; } 
			= new("trust-response-satisfactory", nameof(WasTrustResponseSatisfactory), "Was the trust result satisfactory?")
			{
				RadioItems = new List<SimpleRadioItem>
				{
					new ("Satisfactory", "satisfactory"),
					new ("Not satisfactory", "not-satisfactory")
				},
				SelectedId = "not-satisfactory"
			};
		
		public RadioButtonsUiComponent SRMAOfferedAfterTFF { get; set; } 
			= new("srma-offered-after-tff",  nameof(SRMAOfferedAfterTFF), "SRMA offered after TFF?")
			{
				RadioItems = new List<SimpleRadioItem>
				{
					new ("Yes", "yes"),
					new ("No","no")
				}
			};
		
		public RadioButtonsUiComponent ForecastingToolRanAt { get; set; } 
			= new(ElementRootId: "forecasting-tool-ran-at", Name: nameof(ForecastingToolRanAt), "When was the forecasting tool run?")
			{
				RadioItems = new List<SimpleRadioItem>
				{
					new ( "Current year - Spring" ,"Current-year---Spring"),
					new ("Current year - Summer","Current-year---Summer"),
					new ( "Previous year - Spring","Previous-year---Spring"),
					new ("Previous year - Summer","Previous-year---Summer")
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
				 if (!ModelState.IsValid)
				 {
				 	return Page();
				 }
				
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