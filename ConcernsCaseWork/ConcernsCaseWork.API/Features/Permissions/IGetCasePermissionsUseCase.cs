using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.Features.Permissions;

public interface IGetCasePermissionsUseCase : IUseCaseAsync<(long[] caseIds, UserInfo userInfo), PermissionQueryResponse>
{
}