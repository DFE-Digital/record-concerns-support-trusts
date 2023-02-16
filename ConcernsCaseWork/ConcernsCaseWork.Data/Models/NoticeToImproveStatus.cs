namespace ConcernsCaseWork.Data.Models
{
	public class NoticeToImproveStatus: IAuditable
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsClosingState { get; set; }
    }
}