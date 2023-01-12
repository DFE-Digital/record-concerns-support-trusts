namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record UpdateDecisionOutcomeResponse
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }

		public int DecisionOutcomeId { get; set; }
	}
}
