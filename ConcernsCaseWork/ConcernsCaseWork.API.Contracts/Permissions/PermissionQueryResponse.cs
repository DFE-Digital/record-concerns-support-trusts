using Ardalis.GuardClauses;

namespace ConcernsCaseWork.API.Contracts.Permissions;

public class PermissionQueryResponse
{
	public PermissionQueryResponse()
	{

	}

	public PermissionQueryResponse(IEnumerable<CasePermissionResponse> casePermissionResponses)
	{
		Guard.Against.Null(casePermissionResponses);
		CasePermissionResponses = casePermissionResponses.ToArray();
	}

	public CasePermissionResponse[] CasePermissionResponses { get; set; }
}