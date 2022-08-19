using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concerns.Data.Models
{
    [Table("NTIWarningLetterReason")]
    public class NTIWarningLetterReason
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<NTIWarningLetterReasonMapping> WarningLetterReasonsMapping { get; set; }
    }
}