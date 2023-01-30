namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImproveReason
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<NoticeToImproveReasonMapping> NoticeToImproveReasonsMapping { get; set; }
    }
}