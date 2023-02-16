namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImproveCondition: IAuditable
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ConditionTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual NoticeToImproveConditionType ConditionType { get; set; }
        public virtual ICollection<NoticeToImproveConditionMapping> NoticeToImproveConditionsMapping { get; set; }
    }
}