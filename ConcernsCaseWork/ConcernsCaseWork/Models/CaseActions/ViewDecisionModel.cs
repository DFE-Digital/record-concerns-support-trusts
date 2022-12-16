using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class ViewDecisionModel
	{
		public int DecisionId { get; set; }
		public int ConcernsCaseUrn { get; set; }
		public string CrmEnquiryNumber { get; set; }
		public string RetrospectiveApproval { get; set; }
		public string SubmissionRequired { get; set; }
		public string SubmissionLink { get; set; }
		public string EsfaReceivedRequestDate { get; set; }
		public string TotalAmountRequested { get; set; }
		public List<string> DecisionTypes { get; set; }
		public string SupportingNotes { get; set; }
		public string EditLink { get; set; }
		public string BackLink { get; set; }
		public ViewDecisionOutcomeModel? Outcome { get; set; }
		public bool IsEditable { get; set; }
		public string CreatedDate { get; set; }
		public string ClosedDate { get; set; }
	}
}
