using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models.Concerns.TeamCasework
{
    [Table("ConcernsCaseworkTeamMember", Schema = "concerns")]
    public class ConcernsCaseworkTeamMember
    {
        [Key]
        public Guid TeamMemberId { get; set; }
        public string TeamMember { get; set; }
    }
}
