using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories;

public static class TrustFinancialForecastFactory
{
	public static TrustFinancialForecast CreateDBModel(CreateTrustFinancialForecastRequest createTrustFinancialForecastRequest)
	{
		return new TrustFinancialForecast
		{
			CaseUrn = createTrustFinancialForecastRequest.CaseUrn,
			SRMAOfferedAfterTFF = createTrustFinancialForecastRequest.SRMAOfferedAfterTFF,
			ForecastingToolRanAt = createTrustFinancialForecastRequest.ForecastingToolRanAt,
			WasTrustResponseSatisfactory = createTrustFinancialForecastRequest.WasTrustResponseSatisfactory,
			Notes = createTrustFinancialForecastRequest.Notes,
			SFSOInitialReviewHappenedAt = createTrustFinancialForecastRequest.SFSOInitialReviewHappenedAt,
			TrustRespondedAt = createTrustFinancialForecastRequest.TrustRespondedAt
		};
	} 
}