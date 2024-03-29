﻿using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public class GetDecisionResponse
	{
		public int ConcernsCaseUrn { get; set; }

		public int DecisionId { get; set; }
		public DecisionTypeQuestion[] DecisionTypes { get; set; }
		public decimal TotalAmountRequested { get; set; }
		public string SupportingNotes { get; set; }
		public DateTimeOffset ReceivedRequestDate { get; set; }
		public string SubmissionDocumentLink { get; set; }
		public bool? SubmissionRequired { get; set; }
		public bool? RetrospectiveApproval { get; set; }

		public bool? HasCrmCase { get; set; }

		public string CrmCaseNumber { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
		public DecisionStatus DecisionStatus { get; set; }
		public DateTimeOffset? ClosedAt { get; set; }
		public string Title { get; set; }

		public DecisionOutcome Outcome { get; set; }
		public bool IsEditable { get; set; }
	}
}
