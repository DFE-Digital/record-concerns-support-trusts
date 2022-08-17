using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concerns.Data.Models
{
    [Table("NTIWarningLetterCase", Schema = "sdd")]
    public class NTIWarningLetter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int CaseUrn { get; set; }
        public DateTime? DateLetterSent { get; set; }
        public int? StatusId { get; set; }
        [StringLength(2000)]
        public string Notes { get; set; }
        public int? ClosedStatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual NTIWarningLetterStatus Status { get; set; }

        [ForeignKey(nameof(ClosedStatusId))]
        public virtual NTIWarningLetterStatus ClosedStatus { get; set; }

        public virtual ICollection<NTIWarningLetterReasonMapping> WarningLetterReasonsMapping { get; set; }
        public virtual ICollection<NTIWarningLetterConditionMapping> WarningLetterConditionsMapping { get; set; }
    }
}