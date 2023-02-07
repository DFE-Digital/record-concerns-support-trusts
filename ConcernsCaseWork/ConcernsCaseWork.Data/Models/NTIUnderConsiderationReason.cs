using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
	public class NTIUnderConsiderationReason: IAuditable
    {
	    public int Id { get; set; } 
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<NTIUnderConsiderationReasonMapping> UnderConsiderationReasonsMapping { get; set; }
    }
}