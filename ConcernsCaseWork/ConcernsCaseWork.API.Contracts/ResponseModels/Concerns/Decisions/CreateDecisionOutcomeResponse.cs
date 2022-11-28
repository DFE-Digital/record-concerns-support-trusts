namespace ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions
{
	public class CreateDecisionOutcomeResponse
	{
		public CreateDecisionOutcomeResponse()
		{
		}

		public CreateDecisionOutcomeResponse(long decisionId, long outcomeId)
		{
			DecisionId = decisionId >= 0 ? decisionId : throw new ArgumentOutOfRangeException(nameof(decisionId), "value must be greater than zero");
			OutcomeId = outcomeId >= 0 ? outcomeId : throw new ArgumentOutOfRangeException(nameof(outcomeId), "value must be greater than zero");
		}
		public long OutcomeId { get; set; }
		public long DecisionId { get; set; }
	}
}
