using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove
{
    public class PatchNoticeToImproveRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public int CaseUrn { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DateStarted { get; set; }
        
        [MaxLength(2000)]
        public string Notes { get; set; }
       
        [MaxLength(300)]
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }
        
        [MaxLength(300)]
        public string SumissionDecisionId { get; set; }
        public DateTime? DateNTILifted { get; set; }
        public DateTime? DateNTIClosed { get; set; }

        public ICollection<int> NoticeToImproveReasonsMapping { get; set; }
        public ICollection<int> NoticeToImproveConditionsMapping { get; set; }
    }
}
