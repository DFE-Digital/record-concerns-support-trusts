﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.Teams;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Teams
{
	public class TeamsCachedService : CachedService, ITeamsCachedService
	{
		private readonly ILogger<TeamsCachedService> _logger;
		private readonly ITeamsService _teamsService;

		private const string _cacheKeyPrefix = "Concerns.Teams";
		private const int _cacheExpiryTimeHours = 8;

		public TeamsCachedService(ILogger<TeamsCachedService> logger, ITeamsService teamsService, ICacheProvider cacheProvider)
			: base(Guard.Against.Null(cacheProvider))
		{
			_teamsService = Guard.Against.Null(teamsService);
			_logger = Guard.Against.Null(logger);
		}

		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);
			_logger.LogDebug($"Reading teams cache for ownerId: {ownerId}");

			async Task<ConcernsCaseworkTeamDto> DoWork()
			{
				var cacheKey = GetCacheKey(ownerId);

				ConcernsCaseworkTeamDto teamDto = await this.GetData<ConcernsCaseworkTeamDto>(cacheKey);

				if (teamDto is null)
				{
					_logger.LogDebug($"Team not found in cache..calling Academies API to get team for ownerId: {ownerId}");

					// response can be null if no team has been created so allow that without caching it.
					teamDto = await _teamsService.GetTeam(ownerId);
					await UpdateCacheIfNotNullDto(cacheKey, teamDto);
				}

				return teamDto;
			}

			return DoWork();
		}

		private Task UpdateCacheIfNotNullDto(string cacheKey, ConcernsCaseworkTeamDto teamDto)
		{
			if (teamDto is null)
			{
				_logger.LogDebug($"Empty team so cache update skipped");
				return Task.CompletedTask;
			}

			Guard.Against.NullOrEmpty(cacheKey);
			Guard.Against.Null(teamDto);

			_logger.LogDebug($"Updating cache for team, ownerId: {teamDto.OwnerId}");
			return StoreData(cacheKey, teamDto, _cacheExpiryTimeHours);
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);
			_logger.LogDebug($"Updating teams cache for ownerId: {team.OwnerId}");

			async Task DoWork()
			{
				// Make sure old cached data is removed, then call academies API and cache new data after update.
				await ClearData(team.OwnerId);
				await _teamsService.PutTeam(team);
				await UpdateCacheIfNotNullDto(GetCacheKey(team.OwnerId), team);
			}

			return DoWork();
		}

		public Task<string[]> GetTeamOwners()
		{
			// Not cached..
			return _teamsService.GetTeamOwners();
		}

		public Task<string[]> GetOwnersOfOpenCases()
		{
			// Not cached..
			return _teamsService.GetOwnersOfOpenCases();
		}

		private string GetCacheKey(string ownerId) => $"{_cacheKeyPrefix}.{ownerId}";

		public new Task ClearData(string ownerId)
		{
			Guard.Against.Null(ownerId);
			_logger.LogDebug($"Clearing teams cache for ownerId: {ownerId}");

			async Task DoWork()
			{
				await base.ClearData(GetCacheKey(ownerId));
			}
			return DoWork();
		}
	}
}