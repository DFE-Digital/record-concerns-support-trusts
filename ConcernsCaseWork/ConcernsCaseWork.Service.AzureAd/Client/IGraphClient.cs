using ConcernsCaseWork.API.Contracts.Users;

namespace ConcernsCaseWork.Service.AzureAd.Client;

public interface IGraphClient
{
	Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken);
	Task<ConcernsCaseWorkAdUser> GetUserByEmailAddress(string emailAddress, CancellationToken cancellationToken);
	Task<string[]> GetUserMemberShips(string emailAddress, CancellationToken cancellationToken);
}