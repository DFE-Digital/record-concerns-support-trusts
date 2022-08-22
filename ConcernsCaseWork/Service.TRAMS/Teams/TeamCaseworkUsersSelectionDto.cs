using Newtonsoft.Json;
using System.Xml.Linq;
using System;
using Ardalis.GuardClauses;

namespace Service.TRAMS.Teams
{
	public sealed class TeamCaseworkUsersSelectionDto
	{
		[JsonProperty("ownerId")]

		public string OwnerId { get; }

		[JsonProperty("selectedTeamMembers")]
		public string[] SelectedTeamMembers { get; }

		[JsonConstructor]
		public TeamCaseworkUsersSelectionDto(string ownerId, string[] selectedTeamMembers) => 
			(OwnerId, SelectedTeamMembers) = (Guard.Against.NullOrEmpty(ownerId, nameof(ownerId)), Guard.Against.Null(selectedTeamMembers, nameof(selectedTeamMembers)));
	}
}
