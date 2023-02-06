using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;

namespace ConcernsCaseWork.Data.Models;

public class TrustFinancialForecast: IAuditable
{
	public int Id { get; set;  }
	public int CaseUrn { get; set; }
	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }
	public DateTime? SFSOInitialReviewHappenedAt { get; set; }
	public DateTime? TrustRespondedAt { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public DateTimeOffset? ClosedAt { get; set; }
	public string Notes { get; set; }
}
