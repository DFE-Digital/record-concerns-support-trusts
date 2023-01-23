namespace ConcernsCaseWork.API.Contracts.Permissions
{
	public class PermissionQueryRequest
	{
		/*
		 *
		 *  {
		 *    int[] CaseIds: CasePermissions[<CasePermissionsRequest>] {
		 *      caseId: <id>,
		 *    }
		 *  }
		 */

		public long[] CaseIds { get; set; }
	}
}
