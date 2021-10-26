namespace ConcernsCaseWork.Security
{
	public static class RoleManager
	{
		public static bool UserHasEditCasePrivileges(string caseCreatedBy, string currentLoggedInUser)
		{
			return caseCreatedBy == currentLoggedInUser;
		}
	}
}
