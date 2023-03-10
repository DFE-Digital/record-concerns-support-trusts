using Ardalis.GuardClauses;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace ConcernsCaseWork.Service.AzureAd.Client;

internal class GraphClient : IGraphClient
{
	private readonly IGraphClientSettings _configuration;

	public GraphClient(IGraphClientSettings configuration)
	{
		_configuration = Guard.Against.Null(configuration);
		_graphClient = new Lazy<GraphServiceClient>(CreateGraphClient);
	}

	private Lazy<GraphServiceClient> _graphClient;

	private void AddUserToResults(User user, List<ConcernsCaseWorkAdUser> results)
	{
		if (!string.IsNullOrWhiteSpace(user.Mail))
		{
			results.Add(new ConcernsCaseWorkAdUser { FirstName = user.GivenName, Surname = user.Surname, Email = user.Mail });
		}
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken)
	{
		const int maxPageSize = 999;
		var client = _graphClient.Value;
		List<ConcernsCaseWorkAdUser> results = new();

		var members = await client
			.Groups[groupId].Members
			.GetAsync(rc =>
			{
				rc.QueryParameters.Top = maxPageSize;
				rc.QueryParameters.Count = true;
				rc.QueryParameters.Select = new[] { "givenName, surname, id, mail" };
				rc.Headers.Add("ConsistencyLevel", "eventual");
			}, cancellationToken);

		var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse?>.CreatePageIterator(client, members,
			(member) =>
			{
				AddUserToResults((User)member, results);
				return true;
			},
			(req) =>
			{
				req.Headers.Add("ConsistencyLevel", "eventual");
				return req;
			});

		await pageIterator.IterateAsync(cancellationToken);

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