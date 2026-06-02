using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

namespace ConcernsCaseWork.Data.Models.Decisions.Outcome
{
	public record DecisionOutcomeAuthorizer
	{
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer Id { get; set; }
		public string Name { get; set; }
	}
}
