using ConcernsCaseWork.Security;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Security
{
	[Parallelizable(ParallelScope.All)]
	public class RbacManagerTests
	{
		[Test]
		public async Task WhenGetDefaultUsers_Return_Users()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);
			
			// act
			var defaultUsers = await rbacManager.GetDefaultUsers();

			// assert
			Assert.That(defaultUsers, Is.Not.Null);
		}

		[Test]
		public async Task WhenGetDefaultUsers_Return_Users_Except_For_Exclusions()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();

			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);

			// act
			var defaultUsers = await rbacManager.GetDefaultUsers("user3");

			// assert
			Assert.That(defaultUsers, Is.Not.Null);
			Assert.IsFalse(defaultUsers.Contains("user3"));
		}

		[Test]
		public async Task WhenGetUsersRoles_Return_SortedDicUserRoleClaim()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();

			var userRoles = RoleFactory.BuildDicUsersRoles();
			
			mockUserRoleCachedService.Setup(urc => urc.GetUsersRoleClaim(It.IsAny<string[]>()))
				.ReturnsAsync(userRoles);
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);

			// act
			var usersRoles = await rbacManager.GetUsersRoles();

			// assert
			Assert.That(usersRoles, Is.Not.Null);
			CollectionAssert.AreEqual(userRoles, usersRoles);
		}
		
		[Test]
		public async Task WhenGetUserRoles_Return_RolesEnum()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();

			var userRoles = RoleFactory.BuildDicUsersRoles();
			var roleClaimWrapper = userRoles.First().Value;
			
			mockUserRoleCachedService.Setup(urc => urc.GetRoleClaimWrapper(It.IsAny<string[]>(), It.IsAny<string>()))
				.ReturnsAsync(roleClaimWrapper);
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);

			// act
			var userRoleClaimWrapper = await rbacManager.GetUserRoleClaimWrapper(It.IsAny<string>());

			// assert
			Assert.That(userRoleClaimWrapper, Is.Not.Null);
			Assert.That(userRoleClaimWrapper.Roles, Is.Not.Null);
			CollectionAssert.AreEqual(roleClaimWrapper.Roles, userRoleClaimWrapper.Roles);
		}

		[Test]
		public async Task WhenUpdateUserRoles_Return_Task()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();
			
			mockUserRoleCachedService.Setup(urc => urc.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>(), It.IsAny<IList<string>>()));
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);

			// act
			await rbacManager.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>(), It.IsAny<IList<string>>());

			// assert
			mockUserRoleCachedService.Verify(urc => urc.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>(), It.IsAny<IList<string>>()), Times.Once);
		}

		private static IConfiguration BuildConfiguration()
		{
			var initialData = new Dictionary<string, string> { { "app:username", "user1,user2,user3,user4" } };
			var config = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();
			return config;
		}
	}
}