using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public class GetDecisionResponseDto
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }

		public string Title { get; set; }

		public DecisionStatus DecisionStatus { get; set; }

		public DateTimeOffset? ClosedAt { get; set; }
	}
}
