using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Permissions
{
	public class GetCasePermissionsResponse
	{
		public GetCasePermissionsResponse()
		{
			Permissions = new List<CasePermission>();
		}

		public List<CasePermission> Permissions { get; set;}

		public bool HasEditPermissions()
		{
			return Permissions.Contains(CasePermission.Edit);
		}
	}
}
