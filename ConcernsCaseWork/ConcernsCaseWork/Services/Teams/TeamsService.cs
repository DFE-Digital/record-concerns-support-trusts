using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Models.Teams;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public async Task<TeamCaseworkUsersSelectionModel> GetTeamCaseworkSelectedUsers(string currentUser)
		{
			var result = await _teamsServiceClient.GetTeamCaseworkSelectedUsers(currentUser);
			return _mapper.Map<TeamCaseworkUsersSelectionModel>(result);
		}

		public Task UpdateTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionModel selectedUsers)
		{
			// TODO:
			return Task.CompletedTask;
		}
	}
}
