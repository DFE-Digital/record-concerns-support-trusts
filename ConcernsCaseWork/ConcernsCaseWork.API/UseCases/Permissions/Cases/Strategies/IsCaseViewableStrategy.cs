using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;

/// <summary>
/// A strategy for inspecting a case and user and determining if the user is permitted to view the case.
/// </summary>
public class IsCaseViewableStrategy : ICaseActionPermissionStrategy
{
	/// <summary>
	/// Inspects a case and user and decides if the user should be permitted to view the case.
	/// </summary>
	/// <param name="concernsCase">The case to be considered. If null then a None permission will be returned</param>
	/// <param name="userInfo">The user to be considered. If null then an exception will be raised</param>
	/// <returns></returns>
	public CasePermission GetAllowedActionPermission(ConcernsCase concernsCase, UserInfo userInfo)
	{
		if (concernsCase == null)
		{
			return CasePermission.None;
		}

		Guard.Against.Null(userInfo);

		return CasePermission.View;
	}
}