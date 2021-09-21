using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Type;
using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Type
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
			Assert.That(actualType.Count, Is.EqualTo(13));
			
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
			Assert.That(actualType.Count, Is.EqualTo(13));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<TypeDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockTypeService.Verify(c => c.GetTypes(), Times.Never);
		}
		
		[Test]
		public async Task WhenGetTypeByNameAndDescription_ReturnsTypes_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTypeService = new Mock<ITypeService>();
			var mockLogger = new Mock<ILogger<TypeCachedService>>();

			mockTypeService.Setup(s => s.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto);
			
			var typeCachedService = new TypeCachedService(mockCacheProvider.Object, mockTypeService.Object, mockLogger.Object);

			// act
			var typeStatus = await typeCachedService.GetTypeByNameAndDescription("Compliance", "Compliance: Financial reporting");
			
			// assert
			Assert.That(typeStatus, Is.Not.Null);
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<TypeDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockTypeService.Verify(c => c.GetTypes(), Times.Once);
		}
		
		[TestCase("Compliance", "Compliance: Financial reporting")]
		[TestCase("Compliance", "Compliance: Financial returns")]
		[TestCase("Financial", "Financial: Deficit")]
		[TestCase("Financial", "Financial: Projected deficit / Low future surplus")]
		[TestCase("Financial", "Financial: Cash flow shortfall")]
		[TestCase("Financial", "Financial: Clawback")]
		[TestCase("Force Majeure", "")]
		[TestCase("Force Majeure", null)]
		[TestCase("Governance", "Governance: Governance")]
		[TestCase("Governance", "Governance: Closure")]
		[TestCase("Governance", "Governance: Executive Pay")]
		[TestCase("Governance", "Governance: Safeguarding")]
		[TestCase("Irregularity", "Irregularity: Allegations and self reported concerns")]
		[TestCase("Irregularity", "Irregularity: Related party transactions - in year")]
		public async Task WhenGetTypeByNameAndDescription_ReturnsTypes_CacheIsNotNull(string type, string subType)
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTypeService = new Mock<ITypeService>();
			var mockLogger = new Mock<ILogger<TypeCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>())).
				ReturnsAsync(TypeFactory.BuildListTypeDto);
			
			var typeCachedService = new TypeCachedService(mockCacheProvider.Object, mockTypeService.Object, mockLogger.Object);

			// act
			var actualType = await typeCachedService.GetTypeByNameAndDescription(type, subType);
			
			// assert
			Assert.That(actualType, Is.Not.Null);
			Assert.That(actualType.Name, Is.EqualTo(type));
			Assert.That(actualType.Description, Is.EqualTo(subType ?? string.Empty));

			mockCacheProvider.Verify(c => c.GetFromCache<IList<TypeDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<TypeDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockTypeService.Verify(c => c.GetTypes(), Times.Never);
		}
	}
}