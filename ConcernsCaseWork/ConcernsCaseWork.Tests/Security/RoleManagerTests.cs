using NUnit.Framework;
using static ConcernsCaseWork.Security.RoleManager;

namespace ConcernsCaseWork.Tests.Security
{
	[Parallelizable(ParallelScope.All)]
	public class RoleManagerTests
	{
		[TestCase("User1", "User1")]
		public void WhenUserHasEditCasePrivileges_UserIsCreator_Return_True(string createdBy, string currentUser)
		{
			// act
			var result = UserHasEditCasePrivileges(createdBy, currentUser);
			// assert
			Assert.That(result, Is.EqualTo(true));
		}


		[TestCase("User1", "User2")]
		public void WhenUserHasEditCasePrivileges_UserIsNotCreator_Return_False(string createdBy, string currentUser)
		{
			// act
			var result = UserHasEditCasePrivileges(createdBy, currentUser);
			// assert
			Assert.That(result, Is.EqualTo(false));
		}
	}
}