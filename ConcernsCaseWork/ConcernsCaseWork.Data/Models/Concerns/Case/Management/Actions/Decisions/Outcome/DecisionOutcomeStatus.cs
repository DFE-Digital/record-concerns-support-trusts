namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome
{
	public record DecisionOutcomeStatus
	{
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus Id { get; set; }
		public string Name { get; set; }
	}
}
