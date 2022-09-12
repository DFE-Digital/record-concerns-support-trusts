using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public class TeamsService : AbstractService, ITeamsService
	{
		private readonly ILogger<TeamsService> _logger;

		public TeamsService(IHttpClientFactory clientFactory, ILogger<TeamsService> logger) : base(clientFactory, logger)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);
			return await Get<ConcernsCaseworkTeamDto>($"/{EndpointsVersion}/concerns-team-casework/owner/{ownerId}", false);
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);
			return Put($"/{EndpointsVersion}/concerns-team-casework/owner/{team.OwnerId}", team);
		}

		public async Task<string[]> GetTeamOwners()
		{
			return await Get<string[]>($"/{EndpointsVersion}/concerns-team-casework/owners", true);
		}
	}
}
