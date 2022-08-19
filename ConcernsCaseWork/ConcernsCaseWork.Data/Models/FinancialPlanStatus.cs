using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concerns.Data.Models
{
    [Table("FinancialPlanStatus")]
    public class FinancialPlanStatus
    {
        [Key]
        public long Id { get; set;  }
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set;  }
        public DateTime UpdatedAt { get; set; }
        public bool IsClosedStatus { get; set; }
    }
}
