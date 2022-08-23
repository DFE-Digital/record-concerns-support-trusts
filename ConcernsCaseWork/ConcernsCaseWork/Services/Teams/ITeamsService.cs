using ConcernsCaseWork.Models.Teams;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public interface ITeamsService
	{
		public Task<ConcernsTeamCaseworkModel> GetTeamCaseworkSelectedUsers(string ownerId);
		public Task UpdateCaseworkTeam(ConcernsTeamCaseworkModel selections);
	}
}
