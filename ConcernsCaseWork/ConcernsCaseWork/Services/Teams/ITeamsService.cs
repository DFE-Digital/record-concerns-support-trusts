using ConcernsCaseWork.Models.Teams;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public interface ITeamsService
	{
		public Task<TeamCaseworkUsersSelectionModel> GetTeamCaseworkSelectedUsers(string currentUser);
		public Task UpdateTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionModel selections);
	}
}
