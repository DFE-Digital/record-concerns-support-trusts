namespace ConcernsCaseWork.Service.AzureAd;

/// <summary>
/// Represents a user as known to Azure Active Directory
/// </summary>
public record ConcernsCaseWorkAdUser
{
	public string FirstName { get; init; }
	public string Surname { get; init; }
	public string Email { get; init; }
	
	public bool IsCaseworker{ get; set; }

	public bool IsTeamLeader{ get; set; }

	public bool IsAdmin{ get; set; }
}