using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Pipelines.Sockets.Unofficial.Buffers;
using Service.Redis.Base;
using Service.TRAMS.Teams;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Teams
{
	public class TeamsCachedService : CachedService, ITeamsCachedService
	{
		private readonly ILogger<TeamsCachedService> _logger;
		private readonly ITeamsService _tramsTeamsService;

		private const string CacheKeyPrefix = "Concerns.Teams";
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
			_logger.LogDebug($"Reading teams cache for ownerId: {ownerId}");

			async Task<ConcernsCaseworkTeamDto> DoWork()
			{
				var cacheKey = GetCacheKey(ownerId);

				ConcernsCaseworkTeamDto teamDto = await this.GetData<ConcernsCaseworkTeamDto>(cacheKey);

				if (teamDto is null)
				{
					_logger.LogDebug($"Team not found in cache..calling Academies API to get team for ownerId: {ownerId}");

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
				_logger.LogDebug($"Empty team so cache update skipped");
				return Task.CompletedTask;
			}

			Guard.Against.NullOrEmpty(cacheKey);
			Guard.Against.Null(teamDto);

			_logger.LogDebug($"Updating cache for team, ownerId: {teamDto.OwnerId}");
			return StoreData(cacheKey, teamDto, CacheExpiryTimeHours);
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);
			_logger.LogDebug($"Updating teams cache for ownerId: {team.OwnerId}");

			async Task DoWork()
			{
				// Make sure old cached data is removed, then call academies API and cache new data after update.
				await ClearData(team.OwnerId);
				await _tramsTeamsService.PutTeam(team);
				await UpdateCacheIfNotNullDto(GetCacheKey(team.OwnerId), team);
			}

			return DoWork();
		}

		public Task<string[]> GetTeamOwners()
		{
			// Not cached..
			return _tramsTeamsService.GetTeamOwners();
		}

		private string GetCacheKey(string ownerId) => $"{CacheKeyPrefix}.{ownerId}";

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
