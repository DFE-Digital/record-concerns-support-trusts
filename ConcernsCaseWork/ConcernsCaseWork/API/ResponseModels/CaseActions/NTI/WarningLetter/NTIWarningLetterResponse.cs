namespace Concerns.Data.ResponseModels.CaseActions.NTI.WarningLetter
{
    public class NTIWarningLetterResponse
	{
		public long Id { get; set; }
		public int CaseUrn { get; set; }
		public DateTime? DateLetterSent { get; set; }
		public int? StatusId { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Notes { get; set; }

		public DateTime? UpdatedAt { get; set; }
		public string CreatedBy { get; set; }

		public ICollection<int> WarningLetterReasonsMapping { get; set; }
		public ICollection<int> WarningLetterConditionsMapping { get; set; }
		public int? ClosedStatusId { get; set; }
		public DateTime? ClosedAt { get; set; }
	}
}
