using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Teams
{
	public sealed class ConcernsCaseworkTeamDto
	{
		[JsonProperty("ownerId")]

		public string OwnerId { get; }

		[JsonProperty("teamMembers")]
		public string[] TeamMembers { get; }

		[JsonConstructor]
		public ConcernsCaseworkTeamDto(string ownerId, string[] teamMembers) =>
			(OwnerId, TeamMembers) = (Guard.Against.NullOrEmpty(ownerId, nameof(ownerId)), Guard.Against.Null(teamMembers, nameof(teamMembers)));
	}
}
