using Ardalis.GuardClauses;

namespace ConcernsCaseWork.Models.Teams
{
	public class TeamCaseworkUsersSelectionModel
	{
		public TeamCaseworkUsersSelectionModel(string ownerId, string[] selectedTeamMembers)
		{
			this.OwnerId = Guard.Against.NullOrWhiteSpace(ownerId);
			this.SelectedTeamMembers = Guard.Against.Null(selectedTeamMembers);
		}

		public string[] SelectedTeamMembers { get; private set; }
		public string OwnerId { get; private set; }
	}
}
