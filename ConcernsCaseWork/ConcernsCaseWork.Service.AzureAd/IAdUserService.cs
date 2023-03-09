namespace ConcernsCaseWork.Service.AzureAd;

public interface IAdUserService
{
	/// <summary>
	/// Returns all users associated with the active directory groups used by the application.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <remarks>Users that appear in multiple groups are aggregated into a single result with the role properties appropriately set.</remarks>
	/// <returns>ConcernsCaseWorkAdUser[]</returns>
	Task<ConcernsCaseWorkAdUser[]> GetAllUsers(CancellationToken cancellationToken);
	
	/// <summary>
	/// Gets users associated with the Administrator active directory group.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <remarks>Role properties for other groups will not be set in the returned data.</remarks>
	/// <returns>ConcernsCaseWorkAdUser[]</returns>
	Task<ConcernsCaseWorkAdUser[]> GetAdministrators(CancellationToken cancellationToken);
	
	/// <summary>
	/// Gets users associated with the Caseworker active directory group.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <remarks>Role properties for other groups will not be set in the returned data.</remarks>
	/// <returns>ConcernsCaseWorkAdUser[]</returns>
	Task<ConcernsCaseWorkAdUser[]> GetCaseWorkers(CancellationToken cancellationToken);
	
	/// <summary>
	/// Gets users associated with the Team Leader active directory group.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <remarks>Role properties for other groups will not be set in the returned data.</remarks>
	/// <returns>ConcernsCaseWorkAdUser[]</returns>
	Task<ConcernsCaseWorkAdUser[]> GetTeamLeaders(CancellationToken cancellationToken);
}