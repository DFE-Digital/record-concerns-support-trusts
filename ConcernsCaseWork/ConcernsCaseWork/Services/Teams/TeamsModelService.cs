using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Service.Teams;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Teams
{
	public class TeamsModelService : ITeamsModelService
	{
		private readonly ILogger<TeamsModelService> _logger;
		private readonly IMapper _mapper;
		private readonly ITeamsService _teamsServiceClient;

		public TeamsModelService(ILogger<TeamsModelService> logger,
			IMapper mapper,
			ITeamsService teamsServiceClient)
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
				var response = await _teamsServiceClient.GetTeam(ownerId);

				if (response == null)
				{
					return new ConcernsTeamCaseworkModel(ownerId, Array.Empty<string>());
				}

				return new ConcernsTeamCaseworkModel(response.OwnerId, response.TeamMembers);
			}
			return DoWork();
		}

		public async Task<ConcernsTeamCaseworkModel> CheckMemberExists(string ownerId)
		{
			var response = await _teamsServiceClient.GetTeam(ownerId);

			if (response == null)
			{
				return null;
			}

			return new ConcernsTeamCaseworkModel(response.OwnerId, response.TeamMembers);
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

		public Task<string[]> GetOwnersOfOpenCases()
		{
			_logger.LogMethodEntered();
			return _teamsServiceClient.GetOwnersOfOpenCases();
		}

		public async Task<IList<string>> GetTeamOwners(params string[] excludes)
		{
			// TODO: Integrate the known users from the DB with Azure graph to build up a set of users where we can identify those who aren't in the graph and may have left.
			_logger.LogMethodEntered();
			var users = await _teamsServiceClient.GetTeamOwners();
			return users.Except(excludes).ToArray();
		}
	}
}
