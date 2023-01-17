using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

public class CaseActionPermissionStrategyRoot : ICaseActionPermissionStrategyRoot
{
	private readonly IEnumerable<ICaseActionPermissionStrategy> _caseActionsStrategies;
	public CaseActionPermissionStrategyRoot(IEnumerable<ICaseActionPermissionStrategy> caseActionsStrategies)
	{
		_caseActionsStrategies = caseActionsStrategies;
	}
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