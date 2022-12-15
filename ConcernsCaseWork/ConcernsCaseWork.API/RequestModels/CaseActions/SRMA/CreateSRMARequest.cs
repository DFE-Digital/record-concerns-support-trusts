using ConcernsCaseWork.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.CaseActions.SRMA
{
    public class CreateSRMARequest
    {
		[Required]
		public int Id { get; set; }
		[Required]
        public int CaseUrn { get; set; }
		public DateTime CreatedAt { get; set; }
		
		[StringLength(300)]
		public string CreatedBy { get; set; }
		public DateTime DateOffered { get; set; }
		public DateTime? DateAccepted { get; set; }
		public DateTime? DateReportSentToTrust { get; set; }
		public DateTime? DateVisitStart { get; set; }
		public DateTime? DateVisitEnd { get; set; }
		public SRMAStatus Status { get; set; }
		
		[StringLength(2000)]
		public string Notes { get; set; }
		public SRMAReasonOffered? Reason { get; set; }
	}
}
