using ConcernsCaseWork.Models.Teams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public interface ITeamsModelService
	{
		public Task<ConcernsTeamCaseworkModel> GetCaseworkTeam(string ownerId);

		public Task<ConcernsTeamCaseworkModel> CheckMemberExists(string ownerId);

		public Task UpdateCaseworkTeam(ConcernsTeamCaseworkModel teamCaseworkModel);
		public Task<string[]> GetOwnersOfOpenCases();

		public Task<IList<string>> GetTeamOwners(params string[] excludes);
	}
}
