using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove
{
    public class CreateNoticeToImproveRequest
    {
		[Required]
        public int CaseUrn { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DateStarted { get; set; }
        
        [StringLength(2000)]
        public string Notes { get; set; }

        [StringLength(300)]
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }

        public ICollection<int> NoticeToImproveReasonsMapping { get; set; }
        public ICollection<int> NoticeToImproveConditionsMapping { get; set; }
    }
}
