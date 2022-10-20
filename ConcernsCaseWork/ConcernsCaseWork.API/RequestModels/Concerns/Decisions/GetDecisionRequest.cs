namespace ConcernsCaseWork.API.RequestModels.Concerns.Decisions
{
    public class GetDecisionRequest
    {
        public int ConcernsCaseUrn { get; }
        public int DecisionId { get; }

        public GetDecisionRequest(int concernsCaseUrn, int decisionId)
        {
            _ = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
            _ = decisionId <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : decisionId;

            ConcernsCaseUrn = concernsCaseUrn;
            DecisionId = decisionId;
        }
    }
}
