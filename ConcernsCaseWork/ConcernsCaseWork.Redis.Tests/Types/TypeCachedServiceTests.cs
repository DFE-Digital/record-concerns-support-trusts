using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Service.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.Types
{
	[Parallelizable(ParallelScope.All)]
	public class TypeCachedServiceTests
	{
		[Test]
		public async Task WhenClearData_IsSuccessful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTypeService = new Mock<ITypeService>();
			var mockLogger = new Mock<ILogger<TypeCachedService>>();

			var typeCachedService = new TypeCachedService(mockCacheProvider.Object, mockTypeService.Object, mockLogger.Object);

			// act
			await typeCachedService.ClearData();
		}
		
		[Test]
		public async Task WhenGetTypes_ReturnsTypes_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTypeService = new Mock<ITypeService>();
			var mockLogger = new Mock<ILogger<TypeCachedService>>();

			mockTypeService.Setup(s => s.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto);
			
			var typeCachedService = new TypeCachedService(mockCacheProvider.Object, mockTypeService.Object, mockLogger.Object);

			// act
			var actualType = await typeCachedService.GetTypes();
			
			// assert
			Assert.That(actualType, Is.Not.Null);
			Assert.That(actualType.Count, Is.EqualTo(9));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<TypeDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockTypeService.Verify(c => c.GetTypes(), Times.Once);
		}
		
		[Test]
		public async Task WhenGetTypes_ReturnsTypes_CacheIsNotNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTypeService = new Mock<ITypeService>();
			var mockLogger = new Mock<ILogger<TypeCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>())).
				ReturnsAsync(TypeFactory.BuildListTypeDto);
			
			var typeCachedService = new TypeCachedService(mockCacheProvider.Object, mockTypeService.Object, mockLogger.Object);

			// act
			var actualType = await typeCachedService.GetTypes();
			
			// assert
			Assert.That(actualType, Is.Not.Null);
			Assert.That(actualType.Count, Is.EqualTo(9));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<TypeDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockTypeService.Verify(c => c.GetTypes(), Times.Never);
		}
	}
}