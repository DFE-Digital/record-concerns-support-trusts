namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsRating: IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<ConcernsRecord> FkConcernsRecord { get; set; }
    }
}