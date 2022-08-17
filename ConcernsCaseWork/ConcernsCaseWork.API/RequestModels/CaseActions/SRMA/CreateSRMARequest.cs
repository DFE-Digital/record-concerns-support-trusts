using Concerns.Data.Enums;
using System;
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
		public DateTime DateOffered { get; set; }
		public DateTime? DateAccepted { get; set; }
		public DateTime? DateReportSentToTrust { get; set; }
		public DateTime? DateVisitStart { get; set; }
		public DateTime? DateVisitEnd { get; set; }
		public SRMAStatusEnum Status { get; set; }
		public string Notes { get; set; }
		public SRMAReasonOfferedEnum? Reason { get; set; }
	}
}
