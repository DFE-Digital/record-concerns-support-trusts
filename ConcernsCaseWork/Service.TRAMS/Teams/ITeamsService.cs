using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public interface ITeamsService
	{
		public Task<TeamCaseworkUsersSelectionDto> GetTeamCaseworkSelectedUsers(string ownerId);
		public Task PutTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionDto selections);
		public Task DeleteTeamCaseworkSelections(string ownerId);
	}
}