using ConcernsCaseWork.API.Contracts.Users;

namespace ConcernsCaseWork.Service.AzureAd.Client;

public interface IGraphClient
{
	public Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken);
	public Task<ConcernsCaseWorkAdUser> GetUserByEmailAddress(string emailAddress, CancellationToken cancellationToken);
	public Task<string[]> GetUserMemberShips(string emailAddress, CancellationToken cancellationToken);
}