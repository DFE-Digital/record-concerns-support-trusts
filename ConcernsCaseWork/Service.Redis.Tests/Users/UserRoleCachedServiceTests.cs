using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Users
{
	[Parallelizable(ParallelScope.All)]
	public class UserRoleCachedServiceTests
	{
		[Test]
		public async Task WhenGetUserClaims_FromCache_Return_Claims()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userRoleClaimState = new UserRoleClaimState { UserRoleClaim = RoleFactory.BuildDicUsersRoles() };
			var userName = userRoleClaimState.UserRoleClaim.First().Key;
			var userCredentials = new UserCredentials(userName, userName, userName);
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserRoleClaimState>(It.IsAny<string>())).
				ReturnsAsync(userRoleClaimState);

			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);

			// act
			var userClaimsResponse = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(userClaimsResponse, Is.Not.Null);
			Assert.That(userClaimsResponse, Is.InstanceOf<Claims>());
			Assert.That(userClaimsResponse.Email, Is.EqualTo(userClaimsResponse.Email));
			Assert.That(userClaimsResponse.Id, Is.EqualTo(userClaimsResponse.Id));
		}
		
		[Test]
		public async Task WhenGetUserClaims_CacheIsNull_Return_Claims()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userClaims = new Claims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userCredentials = new UserCredentials("test.test", "test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserRoleClaimState>(It.IsAny<string>())).
				ReturnsAsync((UserRoleClaimState)null);
			
			mockActiveDirectoryService.Setup(ad => ad.GetUserAsync(It.IsAny<UserCredentials>())).
				ReturnsAsync(userClaims);
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			var userClaimsResponse = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(userClaimsResponse, Is.Not.Null);
			Assert.That(userClaimsResponse, Is.InstanceOf<Claims>());
			Assert.That(userClaimsResponse.Email, Is.EqualTo(userClaims.Email));
			Assert.That(userClaimsResponse.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetUserClaims_CacheContainsUser_Return_Claims()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userRoleClaimState = new UserRoleClaimState { UserRoleClaim = RoleFactory.BuildDicUsersRoles() };
			var userName = userRoleClaimState.UserRoleClaim.First().Key;
			var userCredentials = new UserCredentials(userName, userName, userName);
			var userClaims = new Claims
			{
				Email = userName, 
				Id = "test"
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserRoleClaimState>(It.IsAny<string>())).
				ReturnsAsync(userRoleClaimState);
			mockActiveDirectoryService.Setup(ad => ad.GetUserAsync(It.IsAny<UserCredentials>())).
				ReturnsAsync(userClaims);
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			var userClaimsResponse = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(userClaimsResponse, Is.Not.Null);
			Assert.That(userClaimsResponse, Is.InstanceOf<Claims>());
			Assert.That(userClaimsResponse.Email, Is.EqualTo(userClaimsResponse.Email));
			Assert.That(userClaimsResponse.Id, Is.EqualTo(userClaimsResponse.Id));
		}
		
		[Test]
		public async Task WhenGetUserClaims_CacheContainsUser_ClaimsAreNull_Return_Claims()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userRoleClaimState = new UserRoleClaimState { UserRoleClaim = RoleFactory.BuildDicUsersRoles() };
			(string userName, RoleClaimWrapper value) = userRoleClaimState.UserRoleClaim.First();
			value.Claims = null;

			var userCredentials = new UserCredentials(userName, userName, userName);
			var userClaims = new Claims
			{
				Email = userName, 
				Id = "test"
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserRoleClaimState>(It.IsAny<string>())).
				ReturnsAsync(userRoleClaimState);
			mockActiveDirectoryService.Setup(ad => ad.GetUserAsync(It.IsAny<UserCredentials>())).
				ReturnsAsync(userClaims);
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			var userClaimsResponse = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(userClaimsResponse, Is.Not.Null);
			Assert.That(userClaimsResponse, Is.InstanceOf<Claims>());
			Assert.That(userClaimsResponse.Email, Is.EqualTo(userClaimsResponse.Email));
			Assert.That(userClaimsResponse.Id, Is.EqualTo(userClaimsResponse.Id));
		}

		[Test]
		public async Task WhenClearData_Return_Task()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			await cachedUserService.ClearData();

			// assert
			mockCacheProvider.Verify(c => c.ClearCache(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenGetUsersRoleClaim_FromCache_Return_DicUserRoleClaim()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userRoleClaimState = new UserRoleClaimState { UserRoleClaim = RoleFactory.BuildDicUsersRoles() };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserRoleClaimState>(It.IsAny<string>())).
				ReturnsAsync(userRoleClaimState);

			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);

			// act
			var userRoleClaimDic = await cachedUserService.GetUsersRoleClaim(new[] { "user1" });

			// assert
			Assert.That(userRoleClaimDic, Is.Not.Null);
			
			CollectionAssert.AreEqual(
				userRoleClaimState.UserRoleClaim.OrderBy(kv => kv.Key).ToList(),
				userRoleClaimDic.OrderBy(kv => kv.Key).ToList()
			);
		}
		
		[Test]
		public async Task WhenGetUsersRoleClaim_CacheIsNull_Return_DicUserRoleClaim()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);

			// act
			var userRoleClaimDic = await cachedUserService.GetUsersRoleClaim(new[] { "concerns.casework", "user1" });

			// assert
			Assert.That(userRoleClaimDic, Is.Not.Null);
		}
	}
}