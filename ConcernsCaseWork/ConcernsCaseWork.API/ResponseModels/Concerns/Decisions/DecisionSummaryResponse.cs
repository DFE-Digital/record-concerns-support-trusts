using ConcernsCaseWork.Data.Enums.Concerns;

namespace ConcernsCaseWork.API.ResponseModels.Concerns.Decisions
{
    public class DecisionSummaryResponse
    {
        public int ConcernsCaseUrn { get; set; }
        public int DecisionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Title { get; set; }
        public DecisionStatus Status { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }
}