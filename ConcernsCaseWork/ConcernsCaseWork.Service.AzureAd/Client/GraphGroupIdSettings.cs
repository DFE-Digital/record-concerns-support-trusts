using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Service.AzureAd.Client;

public class GraphGroupIdSettings : IGraphGroupIdSettings
{
	public string CaseWorkerGroupId { get; init; }
	public string TeamLeaderGroupId { get; init; }
	public string AdminGroupId { get; init; }

	public GraphGroupIdSettings(IConfiguration configuration)
	{
		CaseWorkerGroupId = configuration["AzureAdGroups:CaseWorkerGroupId"];
		TeamLeaderGroupId = configuration["AzureAdGroups:TeamleaderGroupId"];
		AdminGroupId = configuration["AzureAdGroups:AdminGroupId"];
	}
}