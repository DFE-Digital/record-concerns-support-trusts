using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Service.AzureAd;

public record GraphClientSettings : IGraphClientSettings
{	public string TeamLeaderGroupId {get; init;}
	public string AdminGroupId {get; init;}
	public string GraphEndpointScope {get; init; }
	public string ClientSecret {get; init; }
	public string ClientId {get; init; }
	public string TenantId {get; init; }
	public string CaseWorkerGroupId {get; init; }

	public GraphClientSettings(IConfiguration configuration)
	{
		ClientSecret = configuration["AzureAd:ClientSecret"];
		ClientId = configuration["AzureAd:ClientId"];
		TenantId = configuration["AzureAd:TenantId"];

		CaseWorkerGroupId = configuration["AzureAdGroups:CaseWorkerGroupId"];
		TeamLeaderGroupId = configuration["AzureAdGroups:TeamleaderGroupId"];
		AdminGroupId = configuration["AzureAdGroups:AdminGroupId"];
		GraphEndpointScope = configuration["AzureAdGroups:GraphEndpointScope"];
	}
}