using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concerns.Data.Models
{
    [Table("NTIUnderConsiderationCase")]
    public class NTIUnderConsideration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int CaseUrn { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public int? ClosedStatusId { get; set; }
        [ForeignKey(nameof(ClosedStatusId))]
        public virtual NTIUnderConsiderationStatus ClosedStatus { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public virtual ICollection<NTIUnderConsiderationReasonMapping> UnderConsiderationReasonsMapping { get; set; }
    }
}