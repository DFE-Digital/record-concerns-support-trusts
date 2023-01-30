namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImproveConditionMapping
    {
	    public int Id { get; set; }

        public long NoticeToImproveId { get; set; }
		public virtual NoticeToImprove NoticeToImprove { get; set; }

		public int NoticeToImproveConditionId { get; set; }
		public virtual NoticeToImproveCondition NoticeToImproveCondition { get; set; }
	}
}
