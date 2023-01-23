namespace ConcernsCaseWork.API.Contracts.Permissions;

public class CasePermissionResponse
{
	public long CaseId { get; set; }
	public CasePermission[] Permissions { get; set; }
}