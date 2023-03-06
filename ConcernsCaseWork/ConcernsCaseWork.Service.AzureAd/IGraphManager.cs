namespace ConcernsCaseWork.Service.AzureAd;

public interface IGraphManager
{
	Task<ConcernsCaseWorkAdUser[]> GetAllUsers(CancellationToken cancellationToken);
}