using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

/// <summary>
/// The root permissions strategy. You should call this strategy with a case and user to find out what actions the user is permitted to invoke on the case.
/// </summary>
public class CaseActionPermissionStrategyRoot : ICaseActionPermissionStrategyRoot
{
	private readonly IEnumerable<ICaseActionPermissionStrategy> _caseActionsStrategies;
	public CaseActionPermissionStrategyRoot(IEnumerable<ICaseActionPermissionStrategy> caseActionsStrategies)
	{
		_caseActionsStrategies = caseActionsStrategies;
	}

	/// <summary>
	/// Inspects the case and user and returns all of the actions the user is permitted to invoke. If no permissions then an empty array will be returned.
	/// </summary>
	/// <param name="case"></param>
	/// <param name="userInfo"></param>
	/// <returns></returns>
	public CasePermission[] GetPermittedCaseActions(ConcernsCaseResponse @case, UserInfo userInfo)
	{
		Guard.Against.Null(@case);
		Guard.Against.Null(userInfo);

		List<CasePermission> permissions = new();

		foreach (var strategy in _caseActionsStrategies)
		{
			var permission = strategy.GetAllowedActionPermission(@case, userInfo);
			if (permission > CasePermission.None)
			{
				permissions.Add(permission);
			}
		}

		return permissions.ToArray();
	}

}