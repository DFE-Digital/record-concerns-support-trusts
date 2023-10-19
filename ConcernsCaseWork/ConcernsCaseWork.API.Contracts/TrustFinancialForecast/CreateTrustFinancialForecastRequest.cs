namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public record CreateTrustFinancialForecastRequest
{
	public int CaseUrn { get; set; }
	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }
	public string Notes { get; set; }
	public DateTime? SFSOInitialReviewHappenedAt { get; set; }
	public DateTime? TrustRespondedAt { get; set; }

	public virtual bool IsValid() => CaseUrn > 0 && (Notes?.Length ?? 0) <= TrustFinancialForecastConstants.MaxNotesLength;
}
