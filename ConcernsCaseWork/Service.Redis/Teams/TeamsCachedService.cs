using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Teams;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Teams
{
	public class TeamsCachedService : CachedService, ITeamsCachedService
	{
		private readonly ILogger<TeamsCachedService> _logger;
		private readonly ITeamsService _tramsTeamsService;

		private const string CacheKeyPrefix = "Concerns.Teams";
		private readonly SemaphoreSlim _semaphoreTeams = new SemaphoreSlim(1, 1);
		private readonly ITeamsService teamsService;
		private const int CacheExpiryTimeHours = 8;

		public TeamsCachedService(ILogger<TeamsCachedService> logger, ITeamsService teamsService, ICacheProvider cacheProvider)
			: base(Guard.Against.Null(cacheProvider))
		{
			_tramsTeamsService = Guard.Against.Null(teamsService);
			_logger = Guard.Against.Null(logger);
		}

		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);

			async Task<ConcernsCaseworkTeamDto> DoWork()
			{
				var cacheKey = $"{CacheKeyPrefix}.{ownerId}";

				ConcernsCaseworkTeamDto teamDto = await this.GetData<ConcernsCaseworkTeamDto>(cacheKey);

				if (teamDto is null)
				{
					// response can be null if no team has been created so allow that without caching it.
					teamDto = await _tramsTeamsService.GetTeam(ownerId);
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
				return Task.CompletedTask;
			}

			Guard.Against.NullOrEmpty(cacheKey);
			Guard.Against.Null(teamDto);

			return StoreData(cacheKey, teamDto, CacheExpiryTimeHours);
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);

			async Task DoWork()
			{
				// go through to trams and update redis if successful
				await _tramsTeamsService.PutTeam(team);
				await UpdateCacheIfNotNullDto($"{CacheKeyPrefix}.{team.OwnerId}", team);
				await UpdateCacheIfNotNullDto($"{CacheKeyPrefix}.{team.OwnerId}", team);
			}

			return DoWork();
		}
	}
}
