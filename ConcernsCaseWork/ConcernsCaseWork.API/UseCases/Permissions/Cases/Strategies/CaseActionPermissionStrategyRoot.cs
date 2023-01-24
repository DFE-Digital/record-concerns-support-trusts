using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;

/// <summary>
/// The root permissions strategy. You should call this strategy with a case and user to find out what actions the user is permitted to invoke on the case.
/// </summary>
public class CaseActionPermissionStrategyRoot : ICaseActionPermissionStrategyRoot
{
	private readonly IEnumerable<ICaseActionPermissionStrategy> _caseActionsStrategies;
	public CaseActionPermissionStrategyRoot(IEnumerable<ICaseActionPermissionStrategy> caseActionsStrategies)
	{
		_caseActionsStrategies = Guard.Against.Null(caseActionsStrategies);
	}

	/// <summary>
	/// Inspects the case and user and returns all of the actions the user is permitted to invoke. If no permissions then an empty array will be returned.
	/// </summary>
	/// <param name="concernsCase"></param>
	/// <param name="userInfo"></param>
	/// <returns></returns>
	public CasePermission[] GetPermittedCaseActions(ConcernsCase concernsCase, UserInfo userInfo)
	{
		Guard.Against.Null(concernsCase);
		Guard.Against.Null(userInfo);

		Dictionary<CasePermission, int> permissions = new();

		foreach (var strategy in _caseActionsStrategies)
		{
			var permission = strategy.GetAllowedActionPermission(concernsCase, userInfo);
			if (permission > CasePermission.None)
			{
				if (!permissions.ContainsKey(permission))
				{
					permissions[permission] = 1;
				}
				else
				{
					permissions[permission] = ++permissions[permission];
				}
			}
		}

		if (permissions.Any(x => x.Value > 1))
		{
			throw new InvalidOperationException(
				"One or more strategies returned a duplicate permission. Check that strategies are not being called multiple times and that strategies return unique permissions.");
		}

		return permissions.Keys.ToArray();
	}

}