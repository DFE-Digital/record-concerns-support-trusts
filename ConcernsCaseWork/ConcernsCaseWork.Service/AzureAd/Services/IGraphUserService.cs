using Microsoft.Graph;

namespace ConcernsCaseWork.Service.AzureAd.Services;

public interface IGraphUserService
{
	Task<IEnumerable<User>> GetCaseWorkersAndAdmins();

	Task<IEnumerable<User>> GetCaseWorkers();

	Task<IEnumerable<User>> GetAdmins();
}
