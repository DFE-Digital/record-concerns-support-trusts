using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Logging;
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
	public class AddPageModel : EditableTrustFinancialForecastPageModel
	{
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<AddPageModel> _logger;

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
					ResetPageComponentsOnValidationError();
					return Page();
				}

				var trustFinancialForecast = new CreateTrustFinancialForecastRequest
				{
					CaseUrn = Urn,
					SRMAOfferedAfterTFF = (SRMAOfferedAfterTFF?)SRMAOfferedAfterTFF.SelectedId,
					ForecastingToolRanAt = (ForecastingToolRanAt?)ForecastingToolRanAt.SelectedId,
					WasTrustResponseSatisfactory = (WasTrustResponseSatisfactory?)WasTrustResponseSatisfactory.SelectedId,
					Notes = Notes.Contents,
					SFSOInitialReviewHappenedAt = !SFSOInitialReviewHappenedAt.Date?.IsEmpty() ?? false ? SFSOInitialReviewHappenedAt.Date?.ToDateTimeOffset() : null,
					TrustRespondedAt = !TrustRespondedAt.Date?.IsEmpty() ?? false ? TrustRespondedAt.Date?.ToDateTimeOffset() : null
				};

				await _trustFinancialForecastService.Create(trustFinancialForecast);

				return Redirect($"/case/{Urn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}
	}
}