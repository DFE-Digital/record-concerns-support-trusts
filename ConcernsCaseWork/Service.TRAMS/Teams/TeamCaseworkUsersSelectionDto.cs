using Newtonsoft.Json;
using System.Xml.Linq;
using System;
using Ardalis.GuardClauses;

namespace Service.TRAMS.Teams
{
	public sealed class TeamCaseworkUsersSelectionDto
	{
		[JsonProperty("userName")]

		public string UserName { get; }

		[JsonProperty("selectedTeamMembers")]
		public string[] SelectedTeamMembers { get; }

		[JsonConstructor]
		public TeamCaseworkUsersSelectionDto(string userName, string[] selectedTeamMembers) => 
			(UserName, SelectedTeamMembers) = (Guard.Against.NullOrEmpty(userName, nameof(userName)), Guard.Against.Null(selectedTeamMembers, nameof(selectedTeamMembers)));
	}
}
