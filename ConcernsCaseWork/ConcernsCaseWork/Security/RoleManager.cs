namespace ConcernsCaseWork.Security
{
	public static class RoleManager
	{
		public static bool UserHasEditCasePrivileges(string caseCreatedBy, string currentLoggedInUser)
		{
			var result = currentLoggedInUser.CompareTo(caseCreatedBy);
			return result == 0;
		}
	}
}
