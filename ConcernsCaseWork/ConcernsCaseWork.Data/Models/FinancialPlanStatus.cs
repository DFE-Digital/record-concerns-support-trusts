using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Data.Models
{
	public class FinancialPlanStatus: IAuditable
    {
	    public long Id { get; set;  }
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set;  }
        public DateTime UpdatedAt { get; set; }
        public bool IsClosedStatus { get; set; }
    }
}
