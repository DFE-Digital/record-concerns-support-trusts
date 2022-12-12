using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class SRMAModel : CaseActionModel
	{
		public DateTime	DateOffered { get; set; }
		public DateTime? DateAccepted { get; set; }
		public DateTime? DateReportSentToTrust { get; set; }
		public DateTime? DateVisitStart { get; set; }
		public DateTime? DateVisitEnd { get; set; }
		public SRMAStatus Status { get; set; }
		public string Notes { get; set; }
		public SRMAReasonOffered Reason { get; set; }
		public string CreatedBy { get; set; }

		public SRMAModel()
		{
		}

		public SRMAModel(long id, long caseUrn, DateTime dateOffered, DateTime? dateAccepted, DateTime? dateReportSentToTrust, DateTime? dateVisitStart, DateTime? dateVisitEnd, SRMAStatus status, string notes, SRMAReasonOffered reason, DateTime createdAt, string createdBy) : this() =>
		(Id, CaseUrn, DateOffered, DateAccepted, DateReportSentToTrust, DateVisitStart, DateVisitEnd, Status, Notes, Reason, CreatedAt, CreatedBy) =
		(id, caseUrn, dateOffered, dateAccepted, dateReportSentToTrust, dateVisitStart, dateVisitEnd, status, notes, reason, createdAt, createdBy);
	}
}
