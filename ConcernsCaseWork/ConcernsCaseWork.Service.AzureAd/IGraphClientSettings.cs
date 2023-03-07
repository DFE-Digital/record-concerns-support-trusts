namespace ConcernsCaseWork.Service.AzureAd;

public interface IGraphClientSettings
{
	public string AdminGroupId { get; init; }
	public string CaseWorkerGroupId { get; init; }
	public string ClientId { get; init; }
	public string ClientSecret { get; init; }
	string GraphEndpointScope { get; init; }
	public string TeamLeaderGroupId { get; init; }
	public string TenantId { get; init; }
}