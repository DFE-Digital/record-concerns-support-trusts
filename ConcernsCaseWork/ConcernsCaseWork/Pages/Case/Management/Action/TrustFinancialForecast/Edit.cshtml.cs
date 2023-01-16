using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[BindProperties]
	public class EditPageModel : EditableTrustFinancialForecastPageModel
	{
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<EditPageModel> _logger;
		[BindProperty(SupportsGet = true, Name="Id")] public int TrustFinancialForecastId { get; init; }
		
		public EditPageModel(
			ITrustFinancialForecastService trustFinancialForecastService, 
			ILogger<EditPageModel> logger)
		{
			_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var request = new GetTrustFinancialForecastByIdRequest{ CaseUrn = Urn, TrustFinancialForecastId = TrustFinancialForecastId };
			if (!request.IsValid()) 
			{
				return Page();
			}
			
			var trustFinancialForecast = await _trustFinancialForecastService.GetById(request);
			if (trustFinancialForecast == default)
			{
				return Page();
			}

			LoadPageComponents(trustFinancialForecast);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{ 
				if (!ModelState.IsValid) 
				{
					ResetPageComponentsOnValidationError();
					return Page();
				}

				var request = new UpdateTrustFinancialForecastRequest
				{
					TrustFinancialForecastId = TrustFinancialForecastId,
					CaseUrn = Urn,
					SRMAOfferedAfterTFF = (SRMAOfferedAfterTFF?)SRMAOfferedAfterTFF.SelectedId,
					ForecastingToolRanAt = (ForecastingToolRanAt?)ForecastingToolRanAt.SelectedId,
					WasTrustResponseSatisfactory = (WasTrustResponseSatisfactory?)WasTrustResponseSatisfactory.SelectedId,
					Notes = Notes.Contents,
					SFSOInitialReviewHappenedAt = !SFSOInitialReviewHappenedAt.Date?.IsEmpty() ?? false ? SFSOInitialReviewHappenedAt.Date?.ToDateTimeOffset() : null,
					TrustRespondedAt = !TrustRespondedAt.Date?.IsEmpty() ?? false ? TrustRespondedAt.Date?.ToDateTimeOffset() : null
				};

				if (!request.IsValid())
				{
					return Page();
				}
				
				await _trustFinancialForecastService.Update(request);

				return Redirect($"/case/{Urn}/management");
				
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}

		private void LoadPageComponents(TrustFinancialForecastResponse trustFinancialForecast)
		{
			if (trustFinancialForecast.SFSOInitialReviewHappenedAt.HasValue) 
				SFSOInitialReviewHappenedAt.Date = new OptionalDateModel((DateTimeOffset)trustFinancialForecast.SFSOInitialReviewHappenedAt);
				
			if (trustFinancialForecast.TrustRespondedAt.HasValue) 
				TrustRespondedAt.Date = new OptionalDateModel((DateTimeOffset)trustFinancialForecast.TrustRespondedAt);
				
			if (trustFinancialForecast.ForecastingToolRanAt.HasValue) 
				ForecastingToolRanAt.SelectedId = (int)trustFinancialForecast.ForecastingToolRanAt;
				
			if (trustFinancialForecast.WasTrustResponseSatisfactory.HasValue) 
				WasTrustResponseSatisfactory.SelectedId = (int)trustFinancialForecast.WasTrustResponseSatisfactory;
				
			if (trustFinancialForecast.SRMAOfferedAfterTFF.HasValue) 
				SRMAOfferedAfterTFF.SelectedId = (int)trustFinancialForecast.SRMAOfferedAfterTFF;

			Notes.Contents = trustFinancialForecast.Notes;
		}
	}
}