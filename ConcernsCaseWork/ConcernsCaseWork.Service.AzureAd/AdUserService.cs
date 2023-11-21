﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Users;
using ConcernsCaseWork.Service.AzureAd.Client;

namespace ConcernsCaseWork.Service.AzureAd;

public class AdUserService : IAdUserService
{
	private readonly IGraphClient _graphClient;
	private readonly IGraphGroupIdSettings _configuration;

	public AdUserService(IGraphClient graphClient, IGraphGroupIdSettings configuration)
	{
		_graphClient = Guard.Against.Null(graphClient);
		_configuration = Guard.Against.Null(configuration);
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetAllUsers(CancellationToken cancellationToken)
	{
		Dictionary<string, ConcernsCaseWorkAdUser> results = new();
		ConcernsCaseWorkAdUser[] caseWorkers = await GetCaseWorkers(cancellationToken);
		ConcernsCaseWorkAdUser[] teamLeaders = await GetTeamLeaders(cancellationToken);
		ConcernsCaseWorkAdUser[] admins = await GetAdministrators(cancellationToken);

		results = AppendResults(results, caseWorkers);
		results = AppendResults(results, admins);
		results = AppendResults(results, teamLeaders);

		return results.Values.ToArray();
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetAdministrators(CancellationToken cancellationToken)
	{
		var results = await this._graphClient.GetCaseWorkersByGroupId(_configuration.AdminGroupId, cancellationToken);
		foreach (var result in results)
		{
			result.IsAdmin = true;
		}

		return results;
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetCaseWorkers(CancellationToken cancellationToken)
	{
		var results = await _graphClient.GetCaseWorkersByGroupId(_configuration.CaseWorkerGroupId, cancellationToken);
		foreach (var result in results)
		{
			result.IsCaseworker = true;
		}

		return results;
	}

	public async Task<ConcernsCaseWorkAdUser[]> GetTeamLeaders(CancellationToken cancellationToken)
	{
		var results = await _graphClient.GetCaseWorkersByGroupId(_configuration.TeamLeaderGroupId, cancellationToken);

		foreach (var result in results)
		{
			result.IsTeamLeader = true;
		}

		return results;
	}

	public async Task<ConcernsCaseWorkAdUser> GetUserByEmailAddress(string emailAddress, CancellationToken cancellationToken)
	{
		var user = await _graphClient.GetUserByEmailAddress(emailAddress, cancellationToken);
		var memberships = await _graphClient.GetUserMemberShips(emailAddress, cancellationToken);

		user.IsCaseworker= memberships.Contains(_configuration.CaseWorkerGroupId);
		user.IsTeamLeader = memberships.Contains(_configuration.TeamLeaderGroupId);
		user.IsAdmin = memberships.Contains(_configuration.AdminGroupId);

		return user;
	}

	private Dictionary<string, ConcernsCaseWorkAdUser> AppendResults(Dictionary<string, ConcernsCaseWorkAdUser> results, IEnumerable<ConcernsCaseWorkAdUser> newResults)
	{
		foreach (ConcernsCaseWorkAdUser user in newResults)
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
}