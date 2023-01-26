namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterStatus
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsClosingState { get; set; }
        public string PastTenseName { get; set; }
    }
}