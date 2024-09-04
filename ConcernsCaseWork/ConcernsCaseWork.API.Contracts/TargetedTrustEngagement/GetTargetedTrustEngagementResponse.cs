
namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public class GetTargetedTrustEngagementResponse
	{
		public int Id { get; set; }
		public int CaseUrn { get; set; }
		public DateTimeOffset? EngagementStartDate { get; set; }
		public TargetedTrustEngagementActivity? ActivityId { get; set; }
		public TargetedTrustEngagementActivityType[] ActivityTypes { get; set; }
		public string Notes { get; set; }
		public DateTimeOffset? EngagementEndDate { get; set; }
		public TargetedTrustEngagementOutcome? EngagementOutcomeId { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? DeletedAt { get; set; }
		public string Title { get; set; }
		public bool IsEditable { get; set; }
	}
}
