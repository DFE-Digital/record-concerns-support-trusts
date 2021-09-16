using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Status;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Status
{
	[Parallelizable(ParallelScope.All)]
	public class StatusCachedServiceTests
	{
		[Test]
		public async Task WhenClearData_IsSuccessful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockStatusService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<StatusCachedService>>();

			var statusCachedService = new StatusCachedService(mockCacheProvider.Object, mockStatusService.Object, mockLogger.Object);

			// act
			await statusCachedService.ClearData();
		}
		
		[Test]
		public async Task WhenGetStatuses_ReturnsStatus_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockStatusService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<StatusCachedService>>();

			mockStatusService.Setup(s => s.GetStatuses()).ReturnsAsync(StatusFactory.BuildListStatusDto);
			
			var statusCachedService = new StatusCachedService(mockCacheProvider.Object, mockStatusService.Object, mockLogger.Object);

			// act
			var actualStatus = await statusCachedService.GetStatuses();
			
			// assert
			Assert.That(actualStatus, Is.Not.Null);
			Assert.That(actualStatus.Count, Is.EqualTo(3));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<StatusDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockStatusService.Verify(c => c.GetStatuses(), Times.Once);
		}
		
		[Test]
		public async Task WhenGetStatuses_ReturnsStatus_CacheIsNotNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockStatusService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<StatusCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>())).
				ReturnsAsync(StatusFactory.BuildListStatusDto);
			
			var statusCachedService = new StatusCachedService(mockCacheProvider.Object, mockStatusService.Object, mockLogger.Object);

			// act
			var actualStatus = await statusCachedService.GetStatuses();
			
			// assert
			Assert.That(actualStatus, Is.Not.Null);
			Assert.That(actualStatus.Count, Is.EqualTo(3));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<StatusDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockStatusService.Verify(c => c.GetStatuses(), Times.Never);
		}
		
		[Test]
		public async Task WhenGetStatusByName_ReturnsStatus_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockStatusService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<StatusCachedService>>();

			mockStatusService.Setup(s => s.GetStatuses()).ReturnsAsync(StatusFactory.BuildListStatusDto);
			
			var statusCachedService = new StatusCachedService(mockCacheProvider.Object, mockStatusService.Object, mockLogger.Object);

			// act
			var actualStatus = await statusCachedService.GetStatusByName("Live");
			
			// assert
			Assert.That(actualStatus, Is.Not.Null);
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<StatusDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockStatusService.Verify(c => c.GetStatuses(), Times.Once);
		}
		
		[Test]
		public async Task WhenGetStatusByName_ReturnsStatus_CacheIsNotNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockStatusService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<StatusCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>())).
				ReturnsAsync(StatusFactory.BuildListStatusDto);
			
			var statusCachedService = new StatusCachedService(mockCacheProvider.Object, mockStatusService.Object, mockLogger.Object);

			// act
			var actualStatus = await statusCachedService.GetStatusByName("Monitoring");
			
			// assert
			Assert.That(actualStatus, Is.Not.Null);

			mockCacheProvider.Verify(c => c.GetFromCache<IList<StatusDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<StatusDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockStatusService.Verify(c => c.GetStatuses(), Times.Never);
		}
	}
}