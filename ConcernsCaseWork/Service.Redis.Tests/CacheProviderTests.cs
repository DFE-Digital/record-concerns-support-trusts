using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Service.Redis.Models;
using Service.Redis.Services;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Tests
{
	[Parallelizable(ParallelScope.All)]
	public class CacheProviderTests
	{
		[Test]
		public async Task WhenGetFromCache_IsSuccessful()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);
			var userClaims = new UserClaims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userClaimsSerialize = JsonSerializer.Serialize(userClaims);

			mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult(Encoding.UTF8.GetBytes(userClaimsSerialize)));

			// act
			var cachedUser = await cacheProvider.GetFromCache<UserClaims>(userClaims.Email);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<UserClaims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetFromCache_IsFailure()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);
			
			mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<byte[]>(null));

			// act
			var cachedUser = await cacheProvider.GetFromCache<UserClaims>("test@email.com");

			// assert
			Assert.That(cachedUser, Is.Null);
		}
		
		[Test]
		public async Task WhenSetCache_IsSuccessful()
		{
			// arrange
			const int cacheTimeToLive = 120;
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);
			var userClaims = new UserClaims
			{
				Email = "test@email.com", 
				Id = "test"
			};

			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			mockCache.Setup(c => 
					c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			// act
			await cacheProvider.SetCache(userClaims.Email, userClaims, cacheEntryOptions);
		}
		
		[Test]
		public void WhenSetCache_IsFailure()
		{
			// arrange
			const int cacheTimeToLive = 120;
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);

			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			// act, assert
			Assert.ThrowsAsync<ArgumentNullException>(() => cacheProvider.SetCache(null, default(object), cacheEntryOptions));
		}
		
		[Test]
		public async Task WhenClearCache_IsSuccessful()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);
			
			mockCache.Setup(c => 
					c.RemoveAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			// act
			await cacheProvider.ClearCache("test@email.com");
		}
		
		[Test]
		public async Task WhenClearCacheKeyIsNull_IsFailure()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var cacheProvider = new CacheProvider(mockCache.Object);
			
			mockCache.Setup(c => 
					c.RemoveAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			// act
			Assert.ThrowsAsync<ArgumentNullException>(() => cacheProvider.ClearCache(null));
		}
	}
}