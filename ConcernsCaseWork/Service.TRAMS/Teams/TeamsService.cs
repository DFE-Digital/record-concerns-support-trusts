using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public class TeamsService : AbstractService, ITeamsService
	{
		private readonly ILogger<TeamsService> _logger;

		public TeamsService(IHttpClientFactory clientFactory, ILogger<TeamsService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = Guard.Against.Null(logger);
		}

		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);

			async Task<ConcernsCaseworkTeamDto> DoWork()
			{
				return await Get<ConcernsCaseworkTeamDto>($"/{_endpointsVersion}/concerns-team-casework/owners/{ownerId}", false);
			}

			return DoWork();
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);
			return Put($"/{_endpointsVersion}/concerns-team-casework/owners/{team.OwnerId}", team);
		}

		public async Task<string[]> GetTeamOwners()
		{
			return await Get<string[]>($"/{_endpointsVersion}/concerns-team-casework/owners", false) ?? Array.Empty<string>();
		}
	}
}
