using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record DecisionOutcome
	{
		public DecisionOutcomeResult OutcomeResult { get; set; }

		public decimal TotalAmount { get; set; }

		public DateTimeOffset DecisionMadeDate { get; set; }

		public DateTimeOffset DecisionEffectiveFromDate { get; set; }

		public DecisionOutcomeAuthorizer Authorizer { get; set; }

		public List<BusinessArea> BusinessAreasConsulted { get; set; }
	}
}
