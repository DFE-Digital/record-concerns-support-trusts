namespace ConcernsCaseWork.Data.Models.Concerns.TeamCasework
{
	public class ConcernsCaseworkTeam
    {
        public string Id { get; set; }

        public virtual List<ConcernsCaseworkTeamMember> TeamMembers { get; set; }
    }
}
