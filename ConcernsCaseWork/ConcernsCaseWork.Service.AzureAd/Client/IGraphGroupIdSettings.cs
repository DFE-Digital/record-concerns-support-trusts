namespace ConcernsCaseWork.Service.AzureAd.Client;

public interface IGraphGroupIdSettings
{
	public string AdminGroupId { get; init; }
	public string CaseWorkerGroupId { get; init; }
	public string TeamLeaderGroupId { get; init; }
}