using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

public class IsCaseEditableStrategy: ICaseActionPermissionStrategy
{
	public CasePermission GetAllowedActionPermission(ConcernsCaseResponse @case, UserInfo userInfo)
	{
		if (@case == null || @case.ClosedAt != null)
		{
			return CasePermission.None;
		}

		if (@case.CreatedBy == userInfo.Name || userInfo.IsAdmin())
		{
			return CasePermission.Edit;
		}

		return CasePermission.None;

	}
}