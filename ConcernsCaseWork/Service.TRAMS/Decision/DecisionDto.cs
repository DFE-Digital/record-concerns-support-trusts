using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Service.TRAMS.Decision
{
	public class DecisionDto
	{
		public long DecisionId { get; set; }

		[JsonProperty("ConcernsCaseUrn")]
		public long CaseUrn { get; set; }

		[JsonProperty("CrmCaseNumber")]
		public string CRMEnquiryNumber { get; set; }

		[JsonProperty("RetrospectiveApproval")]
		public bool? RetrospectiveApproval { get; set; }

		[JsonProperty("SubmissionRequired")]
		public bool? SubmissionRequired { get; set; }

		[JsonProperty("SubmissionDocumentLink")]
		public string SubmissionDocumentLink { get; set; }

		[JsonProperty("ReceivedRequestDate")]
		public DateTime? DateEFSAReceivedRequest { get; set; }

		public double TotalAmountRequested { get; set; }

		[JsonProperty("SupportingNotes")]
		public string Notes { get; set; }

		public IEnumerable<int> DecisionTypes { get; set; }

		public DateTime CreatedAt { get; set; }
		//public DateTime UpdatedAt { get; set; }
		//public DateTime? ClosedAt { get; set; }
	}
}

