namespace ConcernsCaseWork.Data.Models.Concerns.TeamCasework
{
	public class ConcernsCaseworkTeamMember: IAuditable
    {
	    public Guid TeamMemberId { get; set; }
        public string TeamMember { get; set; }

    }
}
