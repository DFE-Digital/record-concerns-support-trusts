using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Service.AzureAd.Client;

public sealed record GraphClientSettings : IGraphClientSettings
{
	public string GraphEndpointScope { get; init; }
	public string ClientSecret { get; init; }
	public string ClientId { get; init; }
	public string TenantId { get; init; }

	public GraphClientSettings(IConfiguration configuration)
	{
		ClientSecret = configuration["AzureAd:ClientSecret"];
		ClientId = configuration["AzureAd:ClientId"];
		TenantId = configuration["AzureAd:TenantId"];
		GraphEndpointScope = configuration["AzureAdGroups:GraphEndpointScope"];
	}
}