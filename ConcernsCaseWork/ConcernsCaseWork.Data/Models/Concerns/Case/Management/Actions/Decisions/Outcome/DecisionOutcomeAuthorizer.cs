using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome
{
	public record DecisionOutcomeAuthorizer
	{
		public API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer Id { get; set; }
		public string Name { get; set; }
	}
}
