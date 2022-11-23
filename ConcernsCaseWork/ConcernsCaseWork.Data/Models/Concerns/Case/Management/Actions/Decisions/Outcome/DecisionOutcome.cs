using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome
{
	public record DecisionOutcome
	{
		public int DecisionOutcomeId { get; set; }

		public int DecisionId { get; set; }

		[Required]
		public DecisionOutcomeStatus Status { get; set; }

		public decimal? TotalAmount { get; set; }

		public DateTimeOffset? DecisionMadeDate { get; set; }

		public DateTimeOffset? DecisionEffectiveFromDate { get; set; }

		public DecisionOutcomeAuthorizer? Authorizer { get; set; }

		public List<DecisionOutcomeBusinessArea> BusinessAreasConsulted { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
