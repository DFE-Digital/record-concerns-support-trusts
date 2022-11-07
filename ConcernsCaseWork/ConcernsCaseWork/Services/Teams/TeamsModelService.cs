using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Service.Teams;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public class TeamsModelService : ITeamsModelService
	{
		private readonly ILogger<TeamsModelService> _logger;
		private readonly IMapper _mapper;
		private ITeamsCachedService _teamsServiceClient;

		public TeamsModelService(ILogger<TeamsModelService> logger,
			IMapper mapper,
			ITeamsCachedService teamsServiceClient)
		{
			_logger = Guard.Against.Null(logger, nameof(logger));
			_teamsServiceClient = Guard.Against.Null(teamsServiceClient, nameof(teamsServiceClient));
			_mapper = Guard.Against.Null(mapper, nameof(mapper));
		}

		public Task<ConcernsTeamCaseworkModel> GetCaseworkTeam(string ownerId)
		{
			_logger.LogMethodEntered();
			Guard.Against.NullOrWhiteSpace(ownerId);

			async Task<ConcernsTeamCaseworkModel> DoWork()
			{
				// get the team. If null returned then no team exists so create a new empty one
				var result = (await _teamsServiceClient.GetTeam(ownerId))
					?? new ConcernsCaseworkTeamDto(ownerId, Array.Empty<string>());

				return _mapper.Map<ConcernsTeamCaseworkModel>(result);
			}
			return DoWork();
		}

		public Task UpdateCaseworkTeam(ConcernsTeamCaseworkModel teamCaseworkModel)
		{
			_logger.LogMethodEntered();
			Guard.Against.Null(teamCaseworkModel);

			async Task DoWork()
			{
				await _teamsServiceClient.PutTeam(new ConcernsCaseworkTeamDto(teamCaseworkModel.OwnerId, teamCaseworkModel.TeamMembers));
			}
			return DoWork();
		}
	}
}
