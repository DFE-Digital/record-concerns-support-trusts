namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImproveReasonMapping
    {
	    public int Id { get; set; }

        public long NoticeToImproveId { get; set; }
		public virtual NoticeToImprove NoticeToImprove { get; set; }

		public int NoticeToImproveReasonId { get; set; }
		public virtual NoticeToImproveReason NoticeToImproveReason { get; set; }
	}
}
