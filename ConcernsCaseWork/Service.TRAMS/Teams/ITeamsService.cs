using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public interface ITeamsService
	{
		public Task<TeamCaseworkUsersSelectionDto> GetTeamCaseworkSelectedUsers(string username);
	}
}