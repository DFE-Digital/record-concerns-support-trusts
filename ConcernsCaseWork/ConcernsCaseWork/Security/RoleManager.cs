using System;

namespace ConcernsCaseWork.Security
{
	public static class RoleManager
	{
		public static bool UserHasEditCasePrivileges(string caseCreatedBy, string currentLoggedInUser)
		{
			bool result = caseCreatedBy.Equals(currentLoggedInUser, StringComparison.OrdinalIgnoreCase);
			return result;
		}
	}
}
