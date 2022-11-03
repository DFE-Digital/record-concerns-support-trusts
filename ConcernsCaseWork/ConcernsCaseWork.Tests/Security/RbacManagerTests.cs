using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Security;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Security
{
	[Parallelizable(ParallelScope.All)]
	public class RbacManagerTests
	{
		[Test]
		public async Task WhenGetSystemUsers_Return_Users()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RbacManager>>();
			var mockTeamsService = new Mock<ITeamsCachedService>();
			
			var rbacManager = new RbacManager(mockLogger.Object, mockTeamsService.Object);

			mockTeamsService.Setup(x => x.GetTeamOwners()).ReturnsAsync(new[] { "user.one" });
			
			// act
			var users = await rbacManager.GetSystemUsers();

			// assert
			Assert.IsNotNull(users);
			Assert.AreEqual(1, users.Count);
			Assert.AreEqual("user.one", users[0]);
		}

		[Test]
		public async Task WhenGetSystemUsers_Return_Users_Except_For_Exclusions()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RbacManager>>();
			var mockTeamsService = new Mock<ITeamsCachedService>();

			mockTeamsService.Setup(x => x.GetTeamOwners()).ReturnsAsync(new[] { "user1", "user2", "user3", "user4" });

			var rbacManager = new RbacManager(mockLogger.Object, mockTeamsService.Object);

			// act
			var defaultUsers = await rbacManager.GetSystemUsers("user3");

			// assert
			Assert.That(defaultUsers, Is.Not.Null);
			Assert.IsFalse(defaultUsers.Contains("user3"));
			Assert.IsTrue(defaultUsers.Contains("user1"));
			Assert.IsTrue(defaultUsers.Contains("user2"));
			Assert.IsTrue(defaultUsers.Contains("user4"));
		}
	}
}