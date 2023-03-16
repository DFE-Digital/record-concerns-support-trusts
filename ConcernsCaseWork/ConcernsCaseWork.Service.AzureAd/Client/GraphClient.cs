using Ardalis.GuardClauses;
using Azure.Identity;
using ConcernsCaseWork.API.Contracts.Users;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ConcernsCaseWork.Service.AzureAd.Tests")]
namespace ConcernsCaseWork.Service.AzureAd.Client;
[ExcludeFromCodeCoverage] // excluded because this is a boundary service - it 
internal class GraphClient : IGraphClient
{
	private readonly IGraphClientSettings _configuration;

	private Lazy<GraphServiceClient> _graphClient;

	public GraphClient(IGraphClientSettings configuration)
	{
		_configuration = Guard.Against.Null(configuration);
		_graphClient = new Lazy<GraphServiceClient>(CreateGraphClient);
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

	public async Task<ConcernsCaseWorkAdUser> GetUserByEmailAddress(string emailAddress, CancellationToken cancellationToken)
	{
		Guard.Against.Null(emailAddress);
		var user  = await _graphClient.Value
			.Users[emailAddress]
			.GetAsync(rc =>
			{
				rc.QueryParameters.Select = new[] { "givenName, surname, id, mail" };
				rc.Headers.Add("ConsistencyLevel", "eventual");
			}, cancellationToken);

		return new ConcernsCaseWorkAdUser() { FirstName = user.GivenName, Surname = user.Surname, Email = user.Mail };
	}

	public async Task<string[]> GetUserMemberShips(string emailAddress, CancellationToken cancellationToken)
	{
		var membership = await _graphClient.Value
			.Users[emailAddress]
			.MemberOf
			.GetAsync(rc =>
			{
				rc.QueryParameters.Select = new[] { "givenName, surname, id, mail" };
				rc.Headers.Add("ConsistencyLevel", "eventual");
		}, cancellationToken);

		if (membership != null && membership.Value != null)
		{
			return membership.Value.Select(x => x.Id).ToArray();
		}
		else
		{
			return Array.Empty<string>();
		}
	}

	private void AddUserToResults(User user, List<ConcernsCaseWorkAdUser> results)
	{
		if (!string.IsNullOrWhiteSpace(user.Mail))
		{
			results.Add(new ConcernsCaseWorkAdUser { FirstName = user.GivenName, Surname = user.Surname, Email = user.Mail });
		}
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