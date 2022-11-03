﻿namespace ConcernsCaseWork.Service.Decision
{
	public class GetDecisionResponseDto
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }

		public string Title { get; set; }

		public DecisionStatus Status { get; set; }

		public DateTimeOffset? ClosedAt { get; set; }
	}
}
