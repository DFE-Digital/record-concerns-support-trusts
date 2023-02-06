using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome
{
	public record DecisionOutcome: IAuditable
	{
		public DecisionOutcome()
		{
			BusinessAreasConsulted = new List<DecisionOutcomeBusinessAreaMapping>();
		}

		public int DecisionOutcomeId { get; set; }

		public int DecisionId { get; set; }

		[Required]
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus Status { get; set; }

		public decimal? TotalAmount { get; set; }

		public DateTimeOffset? DecisionMadeDate { get; set; }

		public DateTimeOffset? DecisionEffectiveFromDate { get; set; }

		public API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer? Authorizer { get; set; }

		public List<DecisionOutcomeBusinessAreaMapping> BusinessAreasConsulted { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
