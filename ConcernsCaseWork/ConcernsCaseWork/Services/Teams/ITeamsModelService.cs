using ConcernsCaseWork.Models.Teams;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public interface ITeamsModelService
	{
		public Task<ConcernsTeamCaseworkModel> GetCaseworkTeam(string ownerId);
		public Task UpdateCaseworkTeam(ConcernsTeamCaseworkModel selections);
	}
}
