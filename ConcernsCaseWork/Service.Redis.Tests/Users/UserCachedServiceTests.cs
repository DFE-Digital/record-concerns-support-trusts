using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Users
{
	[Parallelizable(ParallelScope.All)]
	public class UserCachedServiceTests
	{
		[Test]
		public async Task WhenGetUserAsyncFromCache_IsSuccessful()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var cachedUserService = new UserRoleCachedService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
			var userClaims = new Claims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userCredentials = new UserCredentials("test.test", "test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<Claims>(It.IsAny<string>())).
				Returns(Task.FromResult(userClaims));

			// act
			var cachedUser = await cachedUserService.GetUserClaimsAsync(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<Claims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_IsSuccessful()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var cachedUserService = new UserRoleCachedService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
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
			
			// act
			var cachedUser = await cachedUserService.GetUserClaimsAsync(userCredentials);

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
			var cachedUserService = new UserRoleCachedService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
			var userCredentials = new UserCredentials("test.test", "test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<Claims>(It.IsAny<string>())).
				Returns(Task.FromResult<Claims>(null));

			// act
			var cachedUser = await cachedUserService.GetUserClaimsAsync(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Null);
		}
	}
}