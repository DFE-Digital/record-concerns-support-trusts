using ConcernsCaseWork.Service.AzureAd.Factories;
using ConcernsCaseWork.Service.AzureAd.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using User = Microsoft.Graph.User;

namespace ConcernsCaseWork.Service.AzureAd.Services;

[ExcludeFromCodeCoverage(Justification = "Mocking out of GraphServiceClient not possible")]
public class GraphUserService : IGraphUserService
{
	private readonly AzureAdGroupsOptions _azureAdGroupsOptions;
	private readonly GraphServiceClient _client;

	public GraphUserService(IGraphClientFactory graphClientFactory, IOptions<AzureAdGroupsOptions> azureAdGroupsOptions)
	{
		_client = graphClientFactory.Create();
		_azureAdGroupsOptions = azureAdGroupsOptions.Value;
	}

	public async Task<IEnumerable<User>> GetTeamleaders()
	{
		return await GetUsersInGroup(_azureAdGroupsOptions.TeamleaderGroupId);
	}

	public async Task<IEnumerable<User>> GetCaseWorkers()
	{
		return await GetUsersInGroup(_azureAdGroupsOptions.CaseWorkerGroupId);
	}

	public async Task<IEnumerable<User>> GetAdmins()
	{
		return await GetUsersInGroup(_azureAdGroupsOptions.AdminGroupId);
	}

	private async Task<IEnumerable<User>> GetUsersInGroup(Guid groupId)
	{
		List<User> users = new();

		IGroupMembersCollectionWithReferencesPage members = await _client.Groups[groupId.ToString()].Members
		   .Request()
		   .Header("ConsistencyLevel", "eventual")
		   .Select("givenName,surname,id,mail")
		   .GetAsync();

		users.AddRange([.. members.Cast<User>().Where(x => x != null && x.Mail != null)]);

		return users;
	}
}
