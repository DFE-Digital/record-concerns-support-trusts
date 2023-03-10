using Ardalis.GuardClauses;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Users;
using Microsoft.Graph.Groups;
using Microsoft.Graph.GroupSettings;
using Microsoft.Graph.Models;
using Microsoft.Graph.Me.GetMemberGroups;

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

	public async Task<ConcernsCaseWorkAdUser[]> GetCaseWorkersByGroupId(string groupId, CancellationToken cancellationToken)
	{
		const int MaxPageSize = 999;

		List<ConcernsCaseWorkAdUser> results = new();
		Action<User> addUserToResults = user =>
		{
			if (!string.IsNullOrWhiteSpace(user.Mail))
			{
				results.Add(new ConcernsCaseWorkAdUser { FirstName = user.GivenName, Surname = user.Surname, Email = user.Mail });
			}
		};
		
		var response = await _graphClient.Value
			.Groups[groupId].Members
			.GetAsync(rc =>
			{
				rc.QueryParameters.Top = MaxPageSize;
				rc.QueryParameters.Count = true;
				rc.QueryParameters.Select = new[] {"givenName, surname, id, mail"};
				rc.Headers.Add("ConsistencyLevel", "eventual");
			}, cancellationToken);

		if (response != null)
		{
			var pageIterator = PageIterator<User, DirectoryObjectCollectionResponse>
				.CreatePageIterator(_graphClient.Value, response, (user) =>
				{
					addUserToResults(user);
					return true;
				});

			await pageIterator.IterateAsync(cancellationToken);
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