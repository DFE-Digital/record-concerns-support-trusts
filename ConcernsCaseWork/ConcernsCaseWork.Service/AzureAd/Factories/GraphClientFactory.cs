using ConcernsCaseWork.Service.AzureAd.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ConcernsCaseWork.Service.AzureAd.Factories;

public class GraphClientFactory : IGraphClientFactory
{
	private readonly AzureAdOptions _azureAdOptions;

	public GraphClientFactory(IOptions<AzureAdOptions> azureAdOptions)
	{
		_azureAdOptions = azureAdOptions.Value;
	}

	public GraphServiceClient Create()
	{
		IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId.ToString())
		   .WithClientSecret(_azureAdOptions.ClientSecret)
		   .WithAuthority(new Uri(_azureAdOptions.Authority))
		   .Build();

		DelegateAuthenticationProvider provider = new(async requestMessage =>
		{
			AuthenticationResult result = await app.AcquireTokenForClient(_azureAdOptions.Scopes).ExecuteAsync();
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
		});

		return new GraphServiceClient($"{_azureAdOptions.ApiUrl}/V1.0/", provider);
	}
}
