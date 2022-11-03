using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NTIUnderConsiderationReasonMapping", Schema = "concerns")]
	public class NTIUnderConsiderationReasonMapping
	{
		[Key]
		public int Id { get; set; }

        public long NTIUnderConsiderationId { get; set; }
		public virtual NTIUnderConsideration NTIUnderConsideration { get; set; }

		public int NTIUnderConsiderationReasonId { get; set; }
		public virtual NTIUnderConsiderationReason NTIUnderConsiderationReason { get; set; }
	}
}

