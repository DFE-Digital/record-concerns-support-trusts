using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.Features.Permissions;

/// <summary>
/// A strategy for inspecting a case and user and determining if the user is permitted to delete the case & entities.
/// </summary>
public class IsCaseDeletableStrategy : ICaseActionPermissionStrategy
{
	/// <summary>
	/// Inspects the case and user info passed and determines if the user should be permitted to delete the case & entities.
	/// </summary>
	/// <param name="concernsCase">The case to be considered. If null then none permission will be returned</param>
	/// <param name="userInfo">The user info to be considered. If null then an exception will be thrown</param>
	/// <returns></returns>
	public CasePermission GetAllowedActionPermission(ConcernsCase concernsCase, UserInfo userInfo)
	{
		// Deletable if user is paas admin.
		// Otherwise not deletable

		if (concernsCase == null)
			return CasePermission.None;

		Guard.Against.Null(userInfo);

		if (userInfo.IsPaasAdmin())
			return CasePermission.EditAndDelete;

		return CasePermission.None;
	}
}