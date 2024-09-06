namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;

public class TargetedTrustEngagementSummaryResponse
{
	public int CaseUrn { get; set; }
	public int TargetedTrustEngagementId { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public string Title { get; set; }
	public int? Outcome { get; set; }
	public DateTimeOffset? ClosedAt { get; set; }
}