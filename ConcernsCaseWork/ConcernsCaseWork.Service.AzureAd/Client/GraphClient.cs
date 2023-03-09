using Ardalis.GuardClauses;
using Azure.Identity;
using Microsoft.Graph;

namespace ConcernsCaseWork.Service.AzureAd.Client;

internal class GraphClient : IGraphClient
{
	private readonly IGraphClientSettings _configuration;

	public GraphClient(IGraphClientSettings configuration)
	{
		_configuration = Guard.Against.Null(configuration);
		_GraphClient = new Lazy<GraphServiceClient>(CreateGraphClient);
	}

	private Lazy<GraphServiceClient> _GraphClient { get; }

	public async Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken)
	{
		const string MaxPageSize = "999";

		List<ConcernsCaseWorkAdUser> results = new();
		Action<IEnumerable<User>> addUsersToResults = azureAdUser => results.AddRange(
			azureAdUser.Where(m => !string.IsNullOrWhiteSpace(m.Mail))
				.Select(x => new ConcernsCaseWorkAdUser { FirstName = x.GivenName, Surname = x.Surname, Email = x.Mail }));

		List<QueryOption> queryOptions = new() { new QueryOption("$count", "true"), new QueryOption("$top", MaxPageSize) };

		IGroupMembersCollectionWithReferencesPage? members = await _GraphClient.Value.Groups[groupId].Members
			.Request(queryOptions)
			.Header("ConsistencyLevel", "eventual")
			.Select("givenName,surname,id,mail")
			.GetAsync(cancellationToken);

		addUsersToResults(members.CurrentPage.Cast<User>());
		while (members.NextPageRequest != null)
		{
			members = await members.NextPageRequest.GetAsync();
			addUsersToResults(members.CurrentPage.Cast<User>());
		}

		return results.ToArray();
	}

	private GraphServiceClient CreateGraphClient()
	{
		// settings for graph client

		// The client credentials flow requires that you request the
		// /.default scope, and preconfigure your permissions on the
		// app registration in Azure. An administrator must grant consent
		// to those permissions beforehand.
		string[] scopes = { _configuration.GraphEndpointScope };

		// using Azure.Identity;
		TokenCredentialOptions options = new() { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };

		// https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
		ClientSecretCredential clientSecretCredential = new(_configuration.TenantId, _configuration.ClientId, _configuration.ClientSecret, options);

		return new GraphServiceClient(clientSecretCredential, scopes);
	}
}