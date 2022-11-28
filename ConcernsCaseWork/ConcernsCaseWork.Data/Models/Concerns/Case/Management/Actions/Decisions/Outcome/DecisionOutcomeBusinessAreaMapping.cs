using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome
{
	public record DecisionOutcomeBusinessAreaMapping
	{
		public int DecisionOutcomeId { get; set; }
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea DecisionOutcomeBusinessId { get; set; }
	}
}
