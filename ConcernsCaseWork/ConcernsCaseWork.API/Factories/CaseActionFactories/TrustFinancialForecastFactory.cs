using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories;

public static class TrustFinancialForecastFactory
{
	public static TrustFinancialForecastResponse ToResponseModel(this TrustFinancialForecast trustFinancialForecast)
	{
		return new TrustFinancialForecastResponse
		{
			CaseUrn = trustFinancialForecast.CaseUrn,
			ClosedAt = trustFinancialForecast.ClosedAt,
			CreatedAt = trustFinancialForecast.CreatedAt,
			UpdatedAt = trustFinancialForecast.UpdatedAt,
			ForecastingToolRanAt = trustFinancialForecast.ForecastingToolRanAt,
			Notes = trustFinancialForecast.Notes,
			SFSOInitialReviewHappenedAt = trustFinancialForecast.SFSOInitialReviewHappenedAt,
			SRMAOfferedAfterTFF = trustFinancialForecast.SRMAOfferedAfterTFF,
			TrustRespondedAt = trustFinancialForecast.TrustRespondedAt,
			TrustFinancialForecastId = trustFinancialForecast.Id,
			WasTrustResponseSatisfactory = trustFinancialForecast.WasTrustResponseSatisfactory
		};
	}
}