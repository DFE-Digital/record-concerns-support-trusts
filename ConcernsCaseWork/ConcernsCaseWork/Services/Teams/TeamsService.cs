using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Models.Teams;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Teams;
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

		public Task<TeamCaseworkUsersSelectionModel> GetTeamCaseworkSelectedUsers(string currentUser)
		{
			Guard.Against.NullOrEmpty(currentUser);

			async Task<TeamCaseworkUsersSelectionModel> DoWork()
			{
				var result = await _teamsServiceClient.GetTeamCaseworkSelectedUsers(currentUser);
				return _mapper.Map<TeamCaseworkUsersSelectionModel>(result);
			}
			return DoWork();
		}

		public Task UpdateTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionModel selectionModel)
		{
			Guard.Against.Null(selectionModel);

			async Task DoWork() {
				if (selectionModel.SelectedTeamMembers.Length == 0)
				{
					await _teamsServiceClient.DeleteTeamCaseworkSelections(selectionModel.UserName);
				}
				else
				{
					await _teamsServiceClient.PutTeamCaseworkSelectedUsers(_mapper.Map<TeamCaseworkUsersSelectionDto>(selectionModel));
				}
			}
			return DoWork();
		}
	}
}
