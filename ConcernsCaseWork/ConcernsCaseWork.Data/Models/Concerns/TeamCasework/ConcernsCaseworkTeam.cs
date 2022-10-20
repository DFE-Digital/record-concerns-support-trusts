using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models.Concerns.TeamCasework
{
    [Table("ConcernsCaseworkTeam", Schema = "concerns")]
    public class ConcernsCaseworkTeam
    {
        public string Id { get; set; }

        public virtual List<ConcernsCaseworkTeamMember> TeamMembers { get; set; }
    }
}
