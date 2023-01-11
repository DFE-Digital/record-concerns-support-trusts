using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using JetBrains.Annotations;
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
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<AddPageModel> _logger;
		
		[BindProperty(SupportsGet = true)] public int Urn { get; init; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case overview");
		public OptionalDateTimeUiComponent SFSOInitialReviewHappenedAt { get; set; } = BuildSFSOInitialReviewHappenedAtComponent();
		public OptionalDateTimeUiComponent TrustRespondedAt { get; set; } = BuildTrustRespondedAtComponent();
		public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();
		public RadioButtonsUiComponent WasTrustResponseSatisfactory { get; set; } = BuildWasTrustResponseSatisfactoryComponent();
		public RadioButtonsUiComponent SRMAOfferedAfterTFF { get; set; } = BuildSRMAOfferedAfterTFFComponent();
		public RadioButtonsUiComponent ForecastingToolRanAt { get; set; } = BuildForecastingToolRanAtComponent();

		public AddPageModel(
			ITrustFinancialForecastService trustFinancialForecastService, 
			ILogger<AddPageModel> logger)
		{
			_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{ 
				if (!ModelState.IsValid) 
				{
					ForecastingToolRanAt = BuildForecastingToolRanAtComponent(ForecastingToolRanAt.SelectedId);
					SRMAOfferedAfterTFF = BuildSRMAOfferedAfterTFFComponent(SRMAOfferedAfterTFF.SelectedId);
					WasTrustResponseSatisfactory = BuildWasTrustResponseSatisfactoryComponent(WasTrustResponseSatisfactory.SelectedId);

					SFSOInitialReviewHappenedAt = BuildSFSOInitialReviewHappenedAtComponent(SFSOInitialReviewHappenedAt.Date);
					TrustRespondedAt = BuildTrustRespondedAtComponent(TrustRespondedAt.Date);
					Notes = BuildNotesComponent(Notes.Contents);

					return Page();
				}

				var trustFinancialForecast = new CreateTrustFinancialForecastRequest
				{
					CaseUrn = Urn,
					SRMAOfferedAfterTFF = (SRMAOfferedAfterTFF?)SRMAOfferedAfterTFF.SelectedId,
					ForecastingToolRanAt = (ForecastingToolRanAt?)ForecastingToolRanAt.SelectedId,
					WasTrustResponseSatisfactory = (WasTrustResponseSatisfactory?)WasTrustResponseSatisfactory.SelectedId,
					Notes = Notes.Contents,
					SFSOInitialReviewHappenedAt = SFSOInitialReviewHappenedAt.Date?.ToDateTimeOffset(),
					TrustRespondedAt = TrustRespondedAt.Date?.ToDateTimeOffset()
				};

				await _trustFinancialForecastService.PostTrustFinancialForecast(trustFinancialForecast);

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
		
		private static RadioButtonsUiComponent BuildForecastingToolRanAtComponent(int? selectedId = null) 
			=> new(ElementRootId: "forecasting-tool-ran-at", Name: nameof(ForecastingToolRanAt), "When was the forecasting tool run?")
			{
				RadioItems = new SimpleRadioItem[]
				{
					new (API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSpring.Description(), (int)API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSpring),
					new (API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSummer.Description(),(int)API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSummer),
					new (API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSpring.Description(),(int)API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSpring),
					new (API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSummer.Description(),(int)API.Contracts.Enums.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSummer)
				},
				SelectedId = selectedId
			};
		
		private static RadioButtonsUiComponent BuildSRMAOfferedAfterTFFComponent(int? selectedId = null) 
			=> new("srma-offered-after-tff",  nameof(SRMAOfferedAfterTFF), "SRMA offered after TFF?")
			{
				RadioItems = new SimpleRadioItem[]
				{
					new (API.Contracts.Enums.TrustFinancialForecast.SRMAOfferedAfterTFF.Yes.Description(), (int)API.Contracts.Enums.TrustFinancialForecast.SRMAOfferedAfterTFF.Yes),
					new (API.Contracts.Enums.TrustFinancialForecast.SRMAOfferedAfterTFF.No.Description(),(int)API.Contracts.Enums.TrustFinancialForecast.SRMAOfferedAfterTFF.No)
				},
				SelectedId = selectedId
			};
		
		private static RadioButtonsUiComponent BuildWasTrustResponseSatisfactoryComponent(int? selectedId = null) 
			=> new("trust-response-satisfactory", nameof(WasTrustResponseSatisfactory), "Was the trust result satisfactory?")
		{
			RadioItems = new SimpleRadioItem[]
			{
				new (API.Contracts.Enums.TrustFinancialForecast.WasTrustResponseSatisfactory.Satisfactory.Description(), (int)API.Contracts.Enums.TrustFinancialForecast.WasTrustResponseSatisfactory.Satisfactory),
				new (API.Contracts.Enums.TrustFinancialForecast.WasTrustResponseSatisfactory.NonSatisfactory.Description(),(int)API.Contracts.Enums.TrustFinancialForecast.WasTrustResponseSatisfactory.NonSatisfactory)
			},
			SelectedId = selectedId
		};
		
		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("notes", nameof(Notes), "Notes (optional)")
			{
				MaxLength = 2000,
				Contents = contents
			};
		
		private static OptionalDateTimeUiComponent BuildTrustRespondedAtComponent([CanBeNull] OptionalDateModel selectedDate = default)
			=> new ("trust-responded-at", nameof(TrustRespondedAt), "When did the trust respond?")
			{
				Date = selectedDate
			};
		
		private static OptionalDateTimeUiComponent BuildSFSOInitialReviewHappenedAtComponent([CanBeNull] OptionalDateModel selectedDate = default)
			=> new("sfso-initial-review-happened-at", nameof(SFSOInitialReviewHappenedAt), "When did SFSO initial review happen?")
			{
				Date = selectedDate
			};
	}
}