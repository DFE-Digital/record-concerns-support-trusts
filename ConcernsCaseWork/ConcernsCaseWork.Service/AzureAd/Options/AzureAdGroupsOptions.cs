namespace ConcernsCaseWork.Service.AzureAd.Options;

public class AzureAdGroupsOptions
{
	public Guid TeamleaderGroupId { get; set; }

	public string GraphEndpointScope { get; set; }

	public Guid CaseWorkerGroupId { get; set; }

	public Guid AdminGroupId { get; set; }
}
