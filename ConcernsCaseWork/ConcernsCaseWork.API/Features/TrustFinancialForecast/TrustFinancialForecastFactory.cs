using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast;

public static class TrustFinancialForecastFactory
{
	public static TrustFinancialForecastResponse ToResponseModel(this Data.Models.TrustFinancialForecast trustFinancialForecast)
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
