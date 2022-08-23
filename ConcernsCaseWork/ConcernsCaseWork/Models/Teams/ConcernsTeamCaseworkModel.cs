using Ardalis.GuardClauses;

namespace ConcernsCaseWork.Models.Teams
{
	public class ConcernsTeamCaseworkModel
	{
		public ConcernsTeamCaseworkModel(string ownerId, string[] teamMembers)
		{
			this.OwnerId = Guard.Against.NullOrWhiteSpace(ownerId);
			this.TeamMembers = Guard.Against.Null(teamMembers);
		}

		public string[] TeamMembers { get; private set; }
		public string OwnerId { get; private set; }
	}
}
