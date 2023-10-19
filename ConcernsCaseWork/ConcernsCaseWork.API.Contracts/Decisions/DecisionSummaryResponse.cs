using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

namespace ConcernsCaseWork.API.Contracts.Decisions;

public class DecisionSummaryResponse
{
	public int ConcernsCaseUrn { get; set; }
	public int DecisionId { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public string Title { get; set; }
	public DecisionStatus Status { get; set; }
	public DecisionOutcomeStatus? Outcome { get; set; }
	public DateTimeOffset? ClosedAt { get; set; }
}