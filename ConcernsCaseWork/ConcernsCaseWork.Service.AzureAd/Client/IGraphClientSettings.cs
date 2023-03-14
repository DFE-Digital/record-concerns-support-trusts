namespace ConcernsCaseWork.Service.AzureAd.Client;

public interface IGraphClientSettings
{
	public string ClientSecret { get; init; }
	public string ClientId { get; init; }
	public string GraphEndpointScope { get; init; }
	public string TenantId { get; init; }
}