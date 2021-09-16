using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.RecordRatingHistory;
using Service.TRAMS.RecordRatingHistory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Tests.RecordRatingHistory
{
	[Parallelizable(ParallelScope.All)]
	public class RecordRatingHistoryCachedServiceTests
	{
		[Test]
		public async Task WhenPostRecordRatingHistory_ReturnsRecordRatingHistoryDto_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordRatingHistoryService = new Mock<IRecordRatingHistoryService>();
			var mockLogger = new Mock<ILogger<RecordRatingHistoryCachedService>>();

			var expectedRecordRatingHistory = RecordRatingHistoryFactory.BuildRecordRatingHistoryDto();
			
			var recordRatingHistoryCachedService = new RecordRatingHistoryCachedService(
				mockCacheProvider.Object, mockRecordRatingHistoryService.Object, mockLogger.Object);

			// act
			var recordRatingHistoryDto = await recordRatingHistoryCachedService.PostRecordRatingHistory(
				expectedRecordRatingHistory, "testing", 1);

			// assert
			Assert.That(recordRatingHistoryDto, Is.Not.Null);
			Assert.That(recordRatingHistoryDto.CreatedAt, Is.EqualTo(expectedRecordRatingHistory.CreatedAt));
			Assert.That(recordRatingHistoryDto.RatingUrn, Is.EqualTo(expectedRecordRatingHistory.RatingUrn));
			Assert.That(recordRatingHistoryDto.RecordUrn, Is.EqualTo(expectedRecordRatingHistory.RecordUrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordRatingHistoryService.Verify(c => 
				c.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPostRecordRatingHistory_ReturnsRecordRatingHistoryDto_CacheIsNotNull_NewKey()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordRatingHistoryService = new Mock<IRecordRatingHistoryService>();
			var mockLogger = new Mock<ILogger<RecordRatingHistoryCachedService>>();

			var expectedRecordRatingHistory = RecordRatingHistoryFactory.BuildRecordRatingHistoryDto();
			var userState = new UserState();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
			var recordRatingHistoryCachedService = new RecordRatingHistoryCachedService(
				mockCacheProvider.Object, mockRecordRatingHistoryService.Object, mockLogger.Object);

			// act
			var recordRatingHistoryDto = await recordRatingHistoryCachedService.PostRecordRatingHistory(
				expectedRecordRatingHistory, "testing", 1);

			// assert
			Assert.That(recordRatingHistoryDto, Is.Not.Null);
			Assert.That(recordRatingHistoryDto.CreatedAt, Is.EqualTo(expectedRecordRatingHistory.CreatedAt));
			Assert.That(recordRatingHistoryDto.RatingUrn, Is.EqualTo(expectedRecordRatingHistory.RatingUrn));
			Assert.That(recordRatingHistoryDto.RecordUrn, Is.EqualTo(expectedRecordRatingHistory.RecordUrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordRatingHistoryService.Verify(c => 
				c.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPostRecordRatingHistory_ReturnsRecordRatingHistoryDto_CacheIsNotNull_KeyExists()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordRatingHistoryService = new Mock<IRecordRatingHistoryService>();
			var mockLogger = new Mock<ILogger<RecordRatingHistoryCachedService>>();

			var expectedRecordRatingHistory = RecordRatingHistoryFactory.BuildRecordRatingHistoryDto();
			
			var userState = new UserState
			{
				TrustUkPrn = "trust-ukprn",
				CasesDetails =
				{
					{ 1, new CaseWrapper { 
						CaseDto = CaseFactory.BuildCaseDto(), 
						Records =
						{
							{ 1, new RecordWrapper
							{
								RecordDto = RecordFactory.BuildRecordDto(), 
								RecordsRatingHistory = new List<RecordRatingHistoryDto>
								{
									new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1)
								}
							} }
						}
					} }
				}
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
			var recordRatingHistoryCachedService = new RecordRatingHistoryCachedService(
				mockCacheProvider.Object, mockRecordRatingHistoryService.Object, mockLogger.Object);

			// act
			var recordRatingHistoryDto = await recordRatingHistoryCachedService.PostRecordRatingHistory(
				expectedRecordRatingHistory, "testing", 1);

			// assert
			Assert.That(recordRatingHistoryDto, Is.Not.Null);
			Assert.That(recordRatingHistoryDto.CreatedAt, Is.EqualTo(expectedRecordRatingHistory.CreatedAt));
			Assert.That(recordRatingHistoryDto.RatingUrn, Is.EqualTo(expectedRecordRatingHistory.RatingUrn));
			Assert.That(recordRatingHistoryDto.RecordUrn, Is.EqualTo(expectedRecordRatingHistory.RecordUrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordRatingHistoryService.Verify(c => 
				c.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>()), Times.Never);
		}
	}
}