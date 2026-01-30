using Microsoft.Graph;

namespace ConcernsCaseWork.Service.AzureAd.Services;

public interface IGraphUserService
{
	Task<IEnumerable<User>> GetTeamleaders();

	Task<IEnumerable<User>> GetCaseWorkers();

	Task<IEnumerable<User>> GetAdmins();
}
