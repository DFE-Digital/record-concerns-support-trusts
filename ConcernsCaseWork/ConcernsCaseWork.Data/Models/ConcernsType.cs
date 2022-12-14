namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<ConcernsRecord> FkConcernsRecord { get; set; }

        public override string ToString() => 
	        Name + 
            (string.IsNullOrEmpty(Description) 
	            ? ""
	            : ": " + Description);
    }
}