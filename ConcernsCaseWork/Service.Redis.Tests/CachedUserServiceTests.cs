using Moq;
using NUnit.Framework;
using Service.Redis.Models;
using Service.Redis.Services;
using System.Threading.Tasks;

namespace Service.Redis.Tests
{
	[Parallelizable(ParallelScope.All)]
	public class CachedUserServiceTests
	{
		[Test]
		public async Task WhenGetUserAsyncFromCache_IsSuccessful()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var cachedUserService = new CachedUserService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
			var userClaims = new UserClaims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userCredentials = new UserCredentials("test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserClaims>(It.IsAny<string>())).
				Returns(Task.FromResult(userClaims));

			// act
			var cachedUser = await cachedUserService.GetUserAsync(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<UserClaims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_IsSuccessful()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var cachedUserService = new CachedUserService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
			var userClaims = new UserClaims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userCredentials = new UserCredentials("test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserClaims>(It.IsAny<string>())).
				Returns(Task.FromResult<UserClaims>(null));

			mockActiveDirectoryService.Setup(ad => ad.GetUserAsync(It.IsAny<UserCredentials>())).
				Returns(Task.FromResult(userClaims));
			
			// act
			var cachedUser = await cachedUserService.GetUserAsync(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<UserClaims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_IsFailure()
		{
			// arrange
			var mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var cachedUserService = new CachedUserService(mockActiveDirectoryService.Object, mockCacheProvider.Object);
			var userCredentials = new UserCredentials("test@email.com", "password");
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserClaims>(It.IsAny<string>())).
				Returns(Task.FromResult<UserClaims>(null));

			// act
			var cachedUser = await cachedUserService.GetUserAsync(userCredentials);

			// assert
			Assert.That(cachedUser, Is.Null);
		}
	}
}