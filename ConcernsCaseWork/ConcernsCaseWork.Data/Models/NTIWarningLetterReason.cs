namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterReason
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<NTIWarningLetterReasonMapping> WarningLetterReasonsMapping { get; set; }
    }
}