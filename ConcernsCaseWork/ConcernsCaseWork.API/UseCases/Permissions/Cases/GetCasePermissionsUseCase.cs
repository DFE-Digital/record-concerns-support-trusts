using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;

namespace ConcernsCaseWork.API.UseCases.Permissions.Cases;

public class GetCasePermissionsUseCase : IGetCasePermissionsUseCase
{
	private readonly ICaseActionPermissionStrategyRoot _caseActionPermissionStrategyRoot;
	private readonly IConcernsCaseGateway _concernsCaseGateway;

	public GetCasePermissionsUseCase(IConcernsCaseGateway concernsCaseGateway, ICaseActionPermissionStrategyRoot caseActionPermissionStrategyRoot)
	{
		_caseActionPermissionStrategyRoot = Guard.Against.Null(caseActionPermissionStrategyRoot);
		_concernsCaseGateway = Guard.Against.Null(concernsCaseGateway);
	}

	public Task<PermissionQueryResponse> Execute((long[] caseIds, UserInfo userInfo) request, CancellationToken cancellationToken)
	{
		_ = Guard.Against.Null(request);
		_ = Guard.Against.Null(request.caseIds);
		_ = Guard.Against.Null(request.userInfo);

		List<CasePermissionResponse> allCasePermissions = new(request.caseIds.Length);
		foreach (long caseId in request.caseIds)
		{
			// TODO: shouldn't need to cast this to an int, incorrect types need to be sorted out.
			// TODO: optimize query to get all cases requested in one db query
			ConcernsCase @case = _concernsCaseGateway.GetConcernsCaseIncludingRecordsById((int)caseId);

			var permittedCaseActions = _caseActionPermissionStrategyRoot.GetPermittedCaseActions(@case, request.userInfo);
			allCasePermissions.Add(new CasePermissionResponse() { CaseId = caseId, Permissions = permittedCaseActions });
		}

		var response = new PermissionQueryResponse(allCasePermissions);
		return Task.FromResult(response);
	}
}