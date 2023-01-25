using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases;

public interface IGetCasePermissionsUseCase : IUseCaseAsync<(long[] caseIds, UserInfo userInfo), PermissionQueryResponse>
{
}