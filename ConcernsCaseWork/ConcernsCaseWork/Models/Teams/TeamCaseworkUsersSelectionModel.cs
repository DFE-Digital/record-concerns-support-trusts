using Ardalis.GuardClauses;

namespace ConcernsCaseWork.Models.Teams
{
	public class TeamCaseworkUsersSelectionModel
	{
		public TeamCaseworkUsersSelectionModel(string userName, string[] selectedTeamMembers)
		{
			this.UserName = Guard.Against.NullOrWhiteSpace(userName);
			this.SelectedTeamMembers = Guard.Against.Null(selectedTeamMembers);
		}

		public string[] SelectedTeamMembers { get; private set; }
		public string UserName { get; private set; }
	}
}
