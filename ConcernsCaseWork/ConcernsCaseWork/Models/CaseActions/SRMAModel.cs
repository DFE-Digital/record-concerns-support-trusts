using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class SRMAModel : CaseActionModel
	{
		public long Id { get; set; }
		public DateTime	DateOffered { get; set; }
		public DateTime? DateAccepted { get; set; }
		public DateTime DateReportSentToTrust { get; set; }
		public DateTime? DateVisitStart { get; set; }
		public DateTime? DateVisitEnd { get; set; }
		public SRMAStatus Status { get; set; }
		public string Notes { get; set; }
		public SRMAReasonOffered Reason { get; set; }

	}
}
