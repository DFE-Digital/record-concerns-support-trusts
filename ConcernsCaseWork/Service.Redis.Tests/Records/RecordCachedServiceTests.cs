using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Records;
using Service.TRAMS.Records;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Records
{
	[Parallelizable(ParallelScope.All)]
	public class RecordCachedServiceTests
	{
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecordDto_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedRecord = RecordFactory.BuildCreateRecordDto();
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecord = await recordRecordCachedService.PostRecordByCaseUrn(expectedRecord, "testing");

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(expectedRecord.Name));
			Assert.That(actualRecord.Urn, Is.EqualTo(expectedRecord.Urn));
			Assert.That(actualRecord.CreatedAt, Is.Not.Null);
			Assert.That(actualRecord.Description, Is.EqualTo(expectedRecord.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(expectedRecord.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
			Assert.That(actualRecord.Status, Is.EqualTo(expectedRecord.Status));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => 
				c.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Never);
		}
	}
}