namespace Concerns.Data.Models
{
    public class ConcernsRating
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Urn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<ConcernsRecord> FkConcernsRecord { get; set; }
    }
}