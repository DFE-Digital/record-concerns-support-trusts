using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public class TeamsService : ITeamsService
	{
		public async Task<TeamCaseworkUsersSelectionDto> GetTeamCaseworkSelectedUsers(string username)
		{			
			return new TeamCaseworkUsersSelectionDto(username, new string[] { "emma.whitcroft" });						
		}

		public Task PutTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionDto selections)
		{
			throw new NotImplementedException();
		}
	}
}
