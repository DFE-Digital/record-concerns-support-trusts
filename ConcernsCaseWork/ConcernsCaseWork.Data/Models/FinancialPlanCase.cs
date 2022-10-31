using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("FinancialPlanCase", Schema = "concerns")]
    public class FinancialPlanCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set;  }
        public int CaseUrn { get; set; }
        public string Name { get; set;  }
        public long? StatusId { get; set; }
        public DateTime? DatePlanRequested { get; set; }
        public DateTime? DateViablePlanReceived { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Notes { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual FinancialPlanStatus Status { get; set; }
    }
}
