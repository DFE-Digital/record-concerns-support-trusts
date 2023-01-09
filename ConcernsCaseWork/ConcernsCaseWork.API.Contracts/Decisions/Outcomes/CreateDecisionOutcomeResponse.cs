namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record CreateDecisionOutcomeResponse
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }

		public int DecisionOutcomeId { get; set; }
	}
}
