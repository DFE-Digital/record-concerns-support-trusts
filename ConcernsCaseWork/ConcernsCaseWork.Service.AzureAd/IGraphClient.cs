namespace ConcernsCaseWork.Service.AzureAd;

public interface IGraphClient
{
	Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken);
}