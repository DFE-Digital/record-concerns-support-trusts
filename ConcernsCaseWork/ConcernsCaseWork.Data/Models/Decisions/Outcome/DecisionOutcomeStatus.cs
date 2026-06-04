namespace ConcernsCaseWork.Data.Models.Decisions.Outcome
{
	public record DecisionOutcomeStatus
	{
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus Id { get; set; }
		public string Name { get; set; }
	}
}
