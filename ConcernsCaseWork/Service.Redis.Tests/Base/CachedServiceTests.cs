using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Base
{
	[Parallelizable(ParallelScope.All)]
	public class CachedServiceTests
	{
		[Test]
		public async Task WhenStoreData_Return_Successful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			
			var cacheService = new CachedService(mockCacheProvider.Object);

			// act
			await cacheService.StoreData("key", new UserState());

			// assert
			mockCacheProvider.Verify(c => 
				c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}
		
		[Test]
		public async Task WhenGetData_Return_Successful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			
			var cacheService = new CachedService(mockCacheProvider.Object);

			// act
			await cacheService.GetData<UserState>("key");

			// assert
			mockCacheProvider.Verify(c => 
				c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenClearData_Return_Successful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			
			var cacheService = new CachedService(mockCacheProvider.Object);

			// act
			await cacheService.ClearData("key");

			// assert
			mockCacheProvider.Verify(c => 
				c.ClearCache(It.IsAny<string>()), Times.Once);

		}
	}
}