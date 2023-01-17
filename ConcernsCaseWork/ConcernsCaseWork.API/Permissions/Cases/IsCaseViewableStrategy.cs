using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

public class IsCaseViewableStrategy: ICaseActionPermissionStrategy
{
	public CasePermission GetAllowedActionPermission(ConcernsCaseResponse @case, UserInfo userInfo)
	{
		if (@case == null)
		{
			return CasePermission.None;
		}
		else return CasePermission.View;
	}
}