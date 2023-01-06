using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record DecisionOutcome
	{
		public DecisionOutcome()
		{
			BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea>();
		}

		public int DecisionOutcomeId { get; set; }

		[Required]
		[EnumDataType(typeof(DecisionOutcomeStatus), ErrorMessage = "Select a decision outcome status")]
		public DecisionOutcomeStatus Status { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal? TotalAmount { get; set; }

		public DateTimeOffset? DecisionMadeDate { get; set; }

		public DateTimeOffset? DecisionEffectiveFromDate { get; set; }

		[EnumDataType(typeof(DecisionOutcomeAuthorizer))]
		public DecisionOutcomeAuthorizer? Authorizer { get; set; }

		public List<DecisionOutcomeBusinessArea> BusinessAreasConsulted { get; set; }
	}
}
