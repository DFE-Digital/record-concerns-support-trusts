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

	public DateTime? SFSOInitialReviewHappenedAt { get; set; }
	
	public DateTime? TrustRespondedAt { get; set; }
	
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public DateTimeOffset? ClosedAt { get; set; }
}
