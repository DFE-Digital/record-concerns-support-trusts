using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Context;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Teams
{
	public class TeamsService : ConcernsAbstractService, ITeamsService
	{
		private readonly ILogger<TeamsService> _logger;

		public TeamsService(IHttpClientFactory clientFactory, ILogger<TeamsService> logger, ICorrelationContext correlationContext, IUserContextService userContextService) : base(clientFactory, logger, correlationContext, userContextService)
		{
			_logger = Guard.Against.Null(logger);
		}

		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);

			async Task<ConcernsCaseworkTeamDto> DoWork()
			{
				return await Get<ConcernsCaseworkTeamDto>($"/{EndpointsVersion}/concerns-team-casework/owners/{ownerId}", false);
			}

			return DoWork();
		}

		public Task PutTeam(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);
			return Put($"/{EndpointsVersion}/concerns-team-casework/owners/{team.OwnerId}", team);
		}

		public async Task<string[]> GetTeamOwners()
		{
			return await Get<string[]>($"/{EndpointsVersion}/concerns-team-casework/owners", false) ?? Array.Empty<string>();
		}
	}
}
