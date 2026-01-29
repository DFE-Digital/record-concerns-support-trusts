using System.Diagnostics.CodeAnalysis;

namespace ConcernsCaseWork.Service.AzureAd.Options;

[ExcludeFromCodeCoverage(Justification = "This is just a class with properties, no logic")]
public class AzureAdGroupsOptions
{
	public Guid TeamleaderGroupId { get; set; }

	public string GraphEndpointScope { get; set; }

	public Guid CaseWorkerGroupId { get; set; }

	public Guid AdminGroupId { get; set; }
}
