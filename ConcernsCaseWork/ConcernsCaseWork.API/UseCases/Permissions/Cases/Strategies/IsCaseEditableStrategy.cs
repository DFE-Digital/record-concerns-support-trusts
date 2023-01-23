using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;

/// <summary>
/// A strategy for inspecting a case and user and determining if the user is permitted to edit the case.
/// </summary>
public class IsCaseEditableStrategy: ICaseActionPermissionStrategy
{
	/// <summary>
	/// Inspects the case and user info passed and determines if the user should be permitted to edit the case.
	/// </summary>
	/// <param name="concernsCase">The case to be considered. If null then none permission will be returned</param>
	/// <param name="userInfo">The user info to be considered. If null then an exception will be thrown</param>
	/// <returns></returns>
	public CasePermission GetAllowedActionPermission(ConcernsCase concernsCase, UserInfo userInfo)
	{
		// Editable if case is not closed and owned by the user.
		// Editable if case is not closed and owned by another user + current user is admin
		// Otherwise not editable

		if (concernsCase == null || concernsCase.ClosedAt != null)
		{
			return CasePermission.None;
		}

		Guard.Against.Null(userInfo);

		if (concernsCase.CreatedBy.Equals(userInfo.Name, StringComparison.InvariantCultureIgnoreCase) || userInfo.IsAdmin())
		{
			return CasePermission.Edit;
		}

		return CasePermission.None;
	}
}