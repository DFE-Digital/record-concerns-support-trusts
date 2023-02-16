namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterCondition: IAuditable
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ConditionTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual NTIWarningLetterConditionType ConditionType { get; set; }

        public virtual ICollection<NTIWarningLetterConditionMapping> WarningLetterConditionsMapping { get; set; }
    }
}