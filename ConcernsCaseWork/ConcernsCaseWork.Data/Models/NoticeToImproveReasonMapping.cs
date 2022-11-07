using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NoticeToImproveReasonMapping", Schema = "concerns")]
	public class NoticeToImproveReasonMapping
    {
		[Key]
		public int Id { get; set; }

        public long NoticeToImproveId { get; set; }
		public virtual NoticeToImprove NoticeToImprove { get; set; }

		public int NoticeToImproveReasonId { get; set; }
		public virtual NoticeToImproveReason NoticeToImproveReason { get; set; }
	}
}
