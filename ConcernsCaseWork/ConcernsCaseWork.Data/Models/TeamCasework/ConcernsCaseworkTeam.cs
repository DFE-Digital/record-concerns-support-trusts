using System.Text.Json;

namespace ConcernsCaseWork.Data.Models.Concerns.TeamCasework
{
	public class ConcernsCaseworkTeam: IAuditable
    {
        public string Id { get; set; }

        public virtual List<ConcernsCaseworkTeamMember> TeamMembers { get; set; }
    }
}
