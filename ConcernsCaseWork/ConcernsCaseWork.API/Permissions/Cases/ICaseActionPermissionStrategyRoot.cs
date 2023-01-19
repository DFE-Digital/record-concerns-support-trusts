using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;

public interface ICaseActionPermissionStrategyRoot
{
	CasePermission[] GetPermittedCaseActions(ConcernsCaseResponse concernsCase, UserInfo userInfo);
}