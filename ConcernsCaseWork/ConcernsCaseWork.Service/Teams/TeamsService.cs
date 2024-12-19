using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Teams
{
	public class TeamsService(IHttpClientFactory clientFactory, ILogger<TeamsService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService userTokenService) : ConcernsAbstractService(clientFactory, logger, correlationContext, userInfoService, userTokenService), ITeamsService
	{
		private readonly ILogger<TeamsService> _logger = Guard.Against.Null(logger);
		private readonly IClientUserInfoService _userInfoService = Guard.Against.Null(userInfoService);
		private readonly IUserTokenService _userTokenService = Guard.Against.Null(userTokenService);

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

		public async Task<string[]> GetOwnersOfOpenCases()
		{
			return await Get<string[]>($"/{EndpointsVersion}/concerns-team-casework/owners/open-cases", false) ?? Array.Empty<string>();
		}
	}
}
