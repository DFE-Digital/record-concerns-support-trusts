using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models.TeamCasework
{
    [Table("ConcernsCaseworkTeam", Schema = "sdd")]
    public class ConcernsCaseworkTeam
    {
        public string Id { get; set; }

        public virtual List<ConcernsCaseworkTeamMember> TeamMembers { get; set; }
    }
}
