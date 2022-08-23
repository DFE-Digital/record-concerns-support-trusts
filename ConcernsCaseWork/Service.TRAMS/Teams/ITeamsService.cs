using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public interface ITeamsService
	{
		public Task<ConcernsCaseworkTeamDto> GetTeamCaseworkSelectedUsers(string ownerId);
		public Task PutTeamCaseworkSelectedUsers(ConcernsCaseworkTeamDto team);
	}
}