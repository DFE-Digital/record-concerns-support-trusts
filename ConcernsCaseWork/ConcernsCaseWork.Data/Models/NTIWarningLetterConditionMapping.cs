using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NTIWarningLetterConditionMapping", Schema = "concerns")]
	public class NTIWarningLetterConditionMapping
	{
		[Key]
		public int Id { get; set; }

        public long NTIWarningLetterId { get; set; }
		public virtual NTIWarningLetter NTIWarningLetter { get; set; }

		public int NTIWarningLetterConditionId { get; set; }
		public virtual NTIWarningLetterCondition NTIWarningLetterCondition { get; set; }
	}
}
