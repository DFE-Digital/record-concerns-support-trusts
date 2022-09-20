using ConcernsCasework.Service.Teams;
using System.Threading.Tasks;

namespace Service.Redis.Teams
{
	// interface is intended to be used as a decorator for the trams teams service
	public interface ITeamsCachedService : ITeamsService
	{
		public Task ClearData(string ownerId);
	}
}
