using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record DecisionOutcome
	{
		public DecisionOutcome()
		{
			BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea>();
		}

		[Required]
		[EnumDataType(typeof(DecisionOutcomeStatus))]
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
