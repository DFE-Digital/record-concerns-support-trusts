using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Models.Teams;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Teams;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{

	public class TeamsService : ITeamsService
	{
		private readonly ILogger<TeamsService> _logger;
		private readonly IMapper _mapper;
		private Service.TRAMS.Teams.ITeamsService _teamsServiceClient;

		public TeamsService(ILogger<TeamsService> logger,
			IMapper mapper,
			Service.TRAMS.Teams.ITeamsService teamsServiceClient)
		{
			_logger = Guard.Against.Null(logger, nameof(logger));
			_teamsServiceClient = Guard.Against.Null(teamsServiceClient, nameof(teamsServiceClient));
			_mapper = Guard.Against.Null(mapper, nameof(mapper));
		}

		public Task<ConcernsTeamCaseworkModel> GetTeamCaseworkSelectedUsers(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);

			async Task<ConcernsTeamCaseworkModel> DoWork()
			{
				// get the team. If null returned then no team exists so create a new empty one
				var result = (await _teamsServiceClient.GetTeamCaseworkSelectedUsers(ownerId))
					?? new ConcernsCaseworkTeamDto(ownerId, Array.Empty<string>());

				return _mapper.Map<ConcernsTeamCaseworkModel>(result);
			}
			return DoWork();
		}

		public Task UpdateCaseworkTeam(ConcernsTeamCaseworkModel team)
		{
			Guard.Against.Null(team);

			async Task DoWork()
			{
				await _teamsServiceClient.PutTeamCaseworkSelectedUsers(new ConcernsCaseworkTeamDto(team.OwnerId, team.TeamMembers));
			}
			return DoWork();
		}
	}
}
