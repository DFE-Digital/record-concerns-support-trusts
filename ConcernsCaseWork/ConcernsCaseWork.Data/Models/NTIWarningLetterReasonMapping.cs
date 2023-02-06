namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterReasonMapping: IAuditable
	{
		public int Id { get; set; }

        public long NTIWarningLetterId { get; set; }
		public virtual NTIWarningLetter NTIWarningLetter { get; set; }

		public int NTIWarningLetterReasonId { get; set; }
		public virtual NTIWarningLetterReason NTIWarningLetterReason { get; set; }
	}
}
