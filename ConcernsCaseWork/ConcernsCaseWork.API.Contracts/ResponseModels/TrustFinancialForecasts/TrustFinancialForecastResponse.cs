using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;

namespace ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;

public record TrustFinancialForecastResponse
{
	public int TrustFinancialForecastId { get; set; }
	public int CaseUrn { get; set; }

	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }

	public string Notes { get; set; }

	public DateTimeOffset? SFSOInitialReviewHappenedAt { get; set; }
	
	public DateTimeOffset? TrustRespondedAt { get; set; }
	
	public DateTimeOffset? ClosedAt { get; set; }
}