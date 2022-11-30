using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public record UpdateDecisionOutcomeResponse
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }

		public int DecisionOutcomeId { get; set; }
	}
}
