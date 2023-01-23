using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;

public interface ICaseActionPermissionStrategyRoot
{
	CasePermission[] GetPermittedCaseActions(ConcernsCase concernsCase, UserInfo userInfo);
}