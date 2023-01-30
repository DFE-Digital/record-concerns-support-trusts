namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterConditionMapping
	{
		public int Id { get; set; }

        public long NTIWarningLetterId { get; set; }
		public virtual NTIWarningLetter NTIWarningLetter { get; set; }

		public int NTIWarningLetterConditionId { get; set; }
		public virtual NTIWarningLetterCondition NTIWarningLetterCondition { get; set; }
	}
}
