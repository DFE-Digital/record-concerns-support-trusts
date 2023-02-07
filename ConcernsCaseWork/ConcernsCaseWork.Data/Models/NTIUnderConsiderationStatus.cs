namespace ConcernsCaseWork.Data.Models
{
	public class NTIUnderConsiderationStatus: IAuditable
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}