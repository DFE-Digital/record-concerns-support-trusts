namespace ConcernsCaseWork.Data.Models.Decisions.Outcome
{
	public record DecisionOutcomeBusinessAreaMapping
	{
		public int DecisionOutcomeId { get; set; }
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea DecisionOutcomeBusinessId { get; set; }
	}
}
