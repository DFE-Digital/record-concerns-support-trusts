using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImprove: IAuditable
    {
	    public long Id { get; set; }
        public int CaseUrn { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DateStarted { get; set; }
        [StringLength(2000)]
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }
        public string SumissionDecisionId { get; set; }
        public DateTime? DateNTILifted { get; set; }
        public DateTime? DateNTIClosed { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual NoticeToImproveStatus Status { get; set; }

        [ForeignKey(nameof(ClosedStatusId))]
        public virtual NoticeToImproveStatus ClosedStatus { get; set; }

        public virtual ICollection<NoticeToImproveReasonMapping> NoticeToImproveReasonsMapping { get; set; }
        public virtual ICollection<NoticeToImproveConditionMapping> NoticeToImproveConditionsMapping { get; set; }
    }
}