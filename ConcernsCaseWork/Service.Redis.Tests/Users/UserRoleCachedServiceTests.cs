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
		public async Task WhenGetUserAsyncFromCache_IsSuccessful()
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
			var claims = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(claims, Is.Not.Null);
			Assert.That(claims, Is.InstanceOf<Claims>());
			Assert.That(claims.Email, Is.EqualTo(claims.Email));
			Assert.That(claims.Id, Is.EqualTo(claims.Id));
		}
		
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_IsSuccessful()
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
			
			mockCacheProvider.Setup(c => c.GetFromCache<Claims>(It.IsAny<string>())).
				Returns(Task.FromResult<Claims>(null));
			mockCacheProvider.Setup(c => c.CacheTimeToLive()).Returns(24);

			mockActiveDirectoryService.Setup(ad => ad.GetUserAsync(It.IsAny<UserCredentials>())).
				Returns(Task.FromResult(userClaims));
			
			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			var cachedUser = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<Claims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_IsFailure()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<UserRoleCachedService>>();
			
			var userCredentials = new UserCredentials("test.test", "test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<Claims>(It.IsAny<string>())).
				Returns(Task.FromResult<Claims>(null));

			var cachedUserService = new UserRoleCachedService(mockCacheProvider.Object, mockActiveDirectoryService.Object, mockLogger.Object);
			
			// act
			var cachedUser = await cachedUserService.GetUserClaims(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Null);
		}
	}
}