using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public interface ITeamsService
	{
		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId);
		public Task PutTeam(ConcernsCaseworkTeamDto team);
		public Task<string[]> GetTeamOwners();
	}
}