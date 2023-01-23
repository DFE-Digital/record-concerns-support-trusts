using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;

public interface ICaseActionPermissionStrategy
{
	public CasePermission GetAllowedActionPermission(ConcernsCase concernsCase, UserInfo userInfo);
}