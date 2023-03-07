using Azure.Identity;
using Microsoft.Graph;

namespace ConcernsCaseWork.Service.AzureAd;

public class GraphManager : IGraphManager
{
	private readonly IGraphClientSettings _configuration;

	public GraphManager(IGraphClientSettings configuration)
	{
		_configuration = configuration;
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetAllUsers(CancellationToken cancellationToken)
	{
		GraphServiceClient graphClient = CreateGraphClient();

		Dictionary<string, ConcernsCaseWorkAdUser> results = new();
		var caseWorkers = await GetCaseWorkers(graphClient, cancellationToken);
		var teamLeaders = await GetTeamLeaders(graphClient, cancellationToken);
		var admins = await GetAdmins(graphClient, cancellationToken);

		results = AppendResults(results, caseWorkers);
		results = AppendResults(results, admins);
		results = AppendResults(results, teamLeaders);

		return results.Values.ToArray();
	}

	private Dictionary<string, ConcernsCaseWorkAdUser> AppendResults(Dictionary<string, ConcernsCaseWorkAdUser> results, List<ConcernsCaseWorkAdUser> newResults)
	{
		foreach (var user in newResults)
		{
			if (results.ContainsKey(user.Email))
			{
				results[user.Email].IsCaseworker |= user.IsCaseworker;
				results[user.Email].IsTeamLeader |= user.IsTeamLeader;
				results[user.Email].IsAdmin |= user.IsAdmin;
			}
			else
			{
				results.Add(user.Email, user);
			}
		}

		return results;
	}

	private Task<List<ConcernsCaseWorkAdUser>> GetCaseWorkers(GraphServiceClient graphClient, CancellationToken cancellationToken)
	{
		return this.GetCaseWorkersByGroupId(_configuration.CaseWorkerGroupId, graphClient, true, false, false, cancellationToken);
	}

	private Task<List<ConcernsCaseWorkAdUser>> GetTeamLeaders(GraphServiceClient graphClient, CancellationToken cancellationToken)
	{
		return this.GetCaseWorkersByGroupId(_configuration.TeamLeaderGroupId, graphClient, false, true, false, cancellationToken);
	}

	private Task<List<ConcernsCaseWorkAdUser>> GetAdmins(GraphServiceClient graphClient, CancellationToken cancellationToken)
	{
		return this.GetCaseWorkersByGroupId(_configuration.AdminGroupId, graphClient, false, false, true, cancellationToken);
	}

	private async Task<List<ConcernsCaseWorkAdUser>> GetCaseWorkersByGroupId(string groupId, GraphServiceClient graphClient, bool isCaseWorker, bool isTeamLeader, bool isAdmin,  CancellationToken cancellationToken)
	{
		List<QueryOption> queryOptions = new() { new("$count", "true"), new("$top", "999") };
		IGroupMembersCollectionWithReferencesPage? members = await graphClient.Groups[groupId].Members
			.Request(queryOptions)
			.Header("ConsistencyLevel", "eventual")
			.Select("givenName,surname,id,mail")
			.GetAsync(cancellationToken);

		return members.Cast<User>()
			.Where(x => !string.IsNullOrWhiteSpace(x.Mail))
			.Select(x => new ConcernsCaseWorkAdUser() { FirstName = x.GivenName, Surname = x.Surname, Email = x.Mail, IsCaseworker = isCaseWorker, IsTeamLeader = isTeamLeader, IsAdmin = isAdmin})
			.ToList();
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