using ConcernsCaseWork.Security;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Security;
using Service.Redis.Teams;
using Service.Redis.Users;
using Service.TRAMS.Teams;
using System.Collections.Generic;
using System.Linq;
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
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();
			var mockTeamsService = new Mock<ITeamsCachedService>();
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object, mockTeamsService.Object);
			
			// act
			var defaultUsers = await rbacManager.GetSystemUsers();

			// assert
			Assert.That(defaultUsers, Is.Not.Null);
		}

		[Test]
		public async Task WhenGetSystemUsers_Return_Users_Except_For_Exclusions()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();
			var mockTeamsService = new Mock<ITeamsCachedService>();

			mockTeamsService.Setup(x => x.GetTeamOwners()).ReturnsAsync(new[] { "user1", "user2", "user3", "user4" });

			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object, mockTeamsService.Object);

			// act
			var defaultUsers = await rbacManager.GetSystemUsers("user3");

			// assert
			Assert.That(defaultUsers, Is.Not.Null);
			Assert.IsFalse(defaultUsers.Contains("user3"));
			Assert.IsTrue(defaultUsers.Contains("user1"));
			Assert.IsTrue(defaultUsers.Contains("user2"));
			Assert.IsTrue(defaultUsers.Contains("user4"));
		}

		private static IConfiguration BuildConfiguration()
		{
			var initialData = new Dictionary<string, string> { { "app:username", "user1,user2,user3,user4" } };
			var config = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();
			return config;
		}
	}
}