using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Configuration;
using ConcernsCaseWork.Redis.Security;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.Base
{
	[Parallelizable(ParallelScope.All)]
	public class CacheProviderTests
	{
		[Test]
		public async Task WhenGetFromCache_IsSuccessful()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			var userClaims = new Claims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			var userClaimsSerialize = JsonSerializer.Serialize(userClaims);

			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = 120});
			mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult(Encoding.UTF8.GetBytes(userClaimsSerialize)));

			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act
			var cachedUser = await cacheProvider.GetFromCache<Claims>(userClaims.Email);

			// assert
			Assert.That(cachedUser, Is.Not.Null);
			Assert.That(cachedUser, Is.InstanceOf<Claims>());
			Assert.That(cachedUser.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUser.Id, Is.EqualTo(userClaims.Id));
		}
		
		[Test]
		public async Task WhenGetFromCache_IsFailure()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();

			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = 120});
			mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<byte[]>(null));

			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act
			var cachedUser = await cacheProvider.GetFromCache<Claims>("test@email.com");

			// assert
			Assert.That(cachedUser, Is.Null);
		}
		
		[Test]
		public async Task WhenSetCache_IsSuccessful()
		{
			// arrange
			const int cacheTimeToLive = 120;
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			var userClaims = new Claims
			{
				Email = "test@email.com", 
				Id = "test"
			};

			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = cacheTimeToLive});
			mockCache.Setup(c => 
					c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act
			await cacheProvider.SetCache(userClaims.Email, userClaims, cacheEntryOptions);
		}
		
		[Test]
		public void WhenSetCache_IsFailure()
		{
			// arrange
			const int cacheTimeToLive = 120;
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			
			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = cacheTimeToLive});
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act, assert
			Assert.ThrowsAsync<ArgumentNullException>(() => cacheProvider.SetCache(null, default(object), cacheEntryOptions));
		}
		
		[Test]
		public async Task WhenClearCache_IsSuccessful()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			
			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = 120});
			mockCache.Setup(c => 
					c.RemoveAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act
			await cacheProvider.ClearCache("test@email.com");
		}
		
		[Test]
		public void WhenClearCacheKeyIsNull_IsFailure()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			
			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = 120});
			mockCache.Setup(c => 
					c.RemoveAsync(It.IsAny<string>(), CancellationToken.None)).
				Returns(Task.FromResult<object>(null));
			
			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			
			// act
			Assert.ThrowsAsync<ArgumentNullException>(() => cacheProvider.ClearCache(null));
		}

		[Test]
		public void WhenCacheTimeToLive_ReturnDefaultTTL()
		{
			// arrange
			var mockCache = new Mock<IDistributedCache>();
			var mockIOptionsCache = new Mock<IOptions<CacheOptions>>();
			
			mockIOptionsCache.Setup(o => o.Value).Returns(new CacheOptions { TimeToLive = 24});
			
			// act
			var cacheProvider = new CacheProvider(mockCache.Object, mockIOptionsCache.Object);
			var cacheTtl = cacheProvider.CacheTimeToLive();

			// assert
			Assert.That(cacheTtl, Is.EqualTo(24));
		}
	}
}