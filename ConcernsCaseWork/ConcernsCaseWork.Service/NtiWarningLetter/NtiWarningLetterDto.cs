namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public DateTime? DateLetterSent { get; set; }
		public string Notes { get; set; }
		public int? StatusId { get; set; }
		public ICollection<int> WarningLetterReasonsMapping { get; set; }
		public ICollection<int> WarningLetterConditionsMapping { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int? ClosedStatusId { get; set; }
	}
}
