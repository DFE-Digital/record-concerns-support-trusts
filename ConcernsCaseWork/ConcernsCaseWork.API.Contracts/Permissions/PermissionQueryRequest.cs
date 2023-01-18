using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	public class CasePermissionsRequest
	{
		public long CaseId { get; set; }
	}

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

	public class CasePermissionResponse
	{
		public long CaseId { get; set; }
		public CasePermission[] Permissions { get; set; }
	}
}
