namespace ConcernsCaseWork.Service.AzureAd.Client;

public interface IGraphClient
{
	Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken);
}