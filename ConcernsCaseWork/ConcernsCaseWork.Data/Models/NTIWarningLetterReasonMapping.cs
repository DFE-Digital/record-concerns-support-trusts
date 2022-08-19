using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concerns.Data.Models
{
    [Table("NTIWarningLetterReasonMapping")]
	public class NTIWarningLetterReasonMapping
	{
		[Key]
		public int Id { get; set; }

        public long NTIWarningLetterId { get; set; }
		public virtual NTIWarningLetter NTIWarningLetter { get; set; }

		public int NTIWarningLetterReasonId { get; set; }
		public virtual NTIWarningLetterReason NTIWarningLetterReason { get; set; }
	}
}

