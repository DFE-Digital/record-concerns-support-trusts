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
			var rolesEnum = await rbacManager.GetUserRoles(It.IsAny<string>());

			// assert
			Assert.That(rolesEnum, Is.Not.Null);
			CollectionAssert.AreEqual(roleClaimWrapper.Roles, rolesEnum);
		}

		[Test]
		public async Task WhenUpdateUserRoles_Return_Task()
		{
			// arrange
			var mockUserRoleCachedService = new Mock<IUserRoleCachedService>();
			var mockLogger = new Mock<ILogger<RbacManager>>();
			
			mockUserRoleCachedService.Setup(urc => urc.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>()));
			
			var rbacManager = new RbacManager(BuildConfiguration(), mockUserRoleCachedService.Object, mockLogger.Object);

			// act
			await rbacManager.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>());

			// assert
			mockUserRoleCachedService.Verify(urc => urc.UpdateUserRoles(It.IsAny<string>(), It.IsAny<IList<RoleEnum>>()), Times.Once);
		}

		private static IConfiguration BuildConfiguration()
		{
			var initialData = new Dictionary<string, string> { { "app:username", "user1,user2,user3,user4" } };
			var config = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();
			return config;
		}
	}
}