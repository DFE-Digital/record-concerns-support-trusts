using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	public class IndexPageModel : AbstractPageModel
	{
		private ITrustFinancialForecastService _trustFinancialForecastService;
		private ILogger _logger;
		
		[BindProperty(SupportsGet = true)] public int Urn { get; init; }
		[BindProperty(SupportsGet = true, Name="Id")] public int TrustFinancialForecastId { get; init; }

		public string SRMAOfferedAfterTFF { get; set; }
		public string ForecastingToolRanAt { get; set; }
		public string WasTrustResponseSatisfactory { get; set; }
		public string Notes { get; set; }
		public string SFSOInitialReviewHappenedAt { get; set; }
		public string TrustRespondedAt { get; set; }
		public string DateOpened { get; set; }
		public string DateClosed { get; set; }
		public bool IsClosed { get; set; }
		public bool IsEditable { get; set; }

		public IndexPageModel(
			ITrustFinancialForecastService trustFinancialForecastService,
			ILogger<IndexPageModel> logger)
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

			IsClosed = trustFinancialForecast.ClosedAt.HasValue;
			IsEditable = !IsClosed;

			SRMAOfferedAfterTFF = trustFinancialForecast.SRMAOfferedAfterTFF?.Description();
			ForecastingToolRanAt = trustFinancialForecast.ForecastingToolRanAt?.Description();
			WasTrustResponseSatisfactory = trustFinancialForecast.WasTrustResponseSatisfactory?.Description();
			Notes = trustFinancialForecast.Notes;
			DateOpened = DateTimeHelper.ParseToDisplayDate(trustFinancialForecast.CreatedAt);
			SFSOInitialReviewHappenedAt = trustFinancialForecast.SFSOInitialReviewHappenedAt.HasValue
				? DateTimeHelper.ParseToDisplayDate(trustFinancialForecast.SFSOInitialReviewHappenedAt.Value)
				: string.Empty;
			TrustRespondedAt = trustFinancialForecast.TrustRespondedAt.HasValue
				? DateTimeHelper.ParseToDisplayDate(trustFinancialForecast.TrustRespondedAt.Value)
				: string.Empty;
			DateClosed = trustFinancialForecast.ClosedAt.HasValue 
				? DateTimeHelper.ParseToDisplayDate(trustFinancialForecast.ClosedAt.Value)
				: string.Empty;
			
				return Page();
		}
	}
}
