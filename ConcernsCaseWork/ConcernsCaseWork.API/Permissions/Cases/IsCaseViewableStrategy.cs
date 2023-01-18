using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

/// <summary>
/// A strategy for inspecting a case and user and determining if the user is permitted to view the case.
/// </summary>
public class IsCaseViewableStrategy : ICaseActionPermissionStrategy
{
	/// <summary>
	/// Inspects a case and user and decides if the user should be permitted to view the case.
	/// </summary>
	/// <param name="case">The case to be considered. If null then a None permission will be returned</param>
	/// <param name="userInfo">The user to be considered. If null then an exception will be raised</param>
	/// <returns></returns>
	public CasePermission GetAllowedActionPermission(ConcernsCaseResponse @case, UserInfo userInfo)
	{
		if (@case == null)
		{
			return CasePermission.None;
		}

		Guard.Against.Null(userInfo);

		return CasePermission.View;
	}
}