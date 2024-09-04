namespace ConcernsCaseWork.Data.Models
{
	public class TargetedTrustEngagementActivity : IAuditable
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}