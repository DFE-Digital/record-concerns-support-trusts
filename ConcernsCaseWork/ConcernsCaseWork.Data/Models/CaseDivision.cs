using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Data.Models
{
	public class CaseDivision : IAuditable
    {
	    public Division Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}