using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Records;
using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;
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
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecordDto_CacheIsNotNull_NewKey()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedRecord = RecordFactory.BuildCreateRecordDto();
			var userState = new UserState();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
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
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecordDto_CacheIsNotNull_KeyExists()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedRecord = RecordFactory.BuildCreateRecordDto();
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CasesDetails = { { 1, new CaseWrapper { CaseDto = CaseFactory.BuildCaseDto(), } } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
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
		
		[Test]
		public async Task WhenGetRecordsByCaseUrn_ReturnsRecordsDto_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(CaseFactory.BuildCaseDto());

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(0));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => 
				c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenGetRecordsByCaseUrn_ReturnsRecordsDto_CacheIsNotNull_KeyExists()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

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
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(CaseFactory.BuildCaseDto());

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(1));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => 
				c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenGetRecordsByCaseUrn_ReturnsRecordsDto_CacheIsNotNull_KeyMissing()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var userState = new UserState
			{
				TrustUkPrn = "trust-ukprn",
				CasesDetails =
				{
					{ 99, new CaseWrapper { 
						CaseDto = CaseFactory.BuildCaseDto(), 
						Records =
						{
							{ 99, new RecordWrapper
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
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(CaseFactory.BuildCaseDto());

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(0));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => 
				c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsTask_CacheIsNotNull_KeyExists()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

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
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			await recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing");

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsTask_CacheIsNotNull_KeyMissing()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var userState = new UserState
			{
				TrustUkPrn = "trust-ukprn",
				CasesDetails =
				{
					{ 99, new CaseWrapper { 
						CaseDto = CaseFactory.BuildCaseDto(), 
						Records =
						{
							{ 99, new RecordWrapper
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
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			await recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing");
			var cachedUserState = await mockCacheProvider.Object.GetFromCache<UserState>(It.IsAny<string>());
			
			// assert
			var caseDetails = cachedUserState.CasesDetails.TryGetValue(99, out var caseWrapper);
			
			Assert.That(caseDetails, Is.True);
			Assert.That(caseWrapper, Is.Not.Null);
			Assert.That(caseWrapper.Records, Is.Not.Null);
			
			caseWrapper.Records.Add(999, new RecordWrapper
			{
				RecordDto = RecordFactory.BuildRecordDto(), 
				RecordsRatingHistory = new List<RecordRatingHistoryDto>
				{
					new RecordRatingHistoryDto(DateTimeOffset.Now, 2, 1)
				}
			});
			
			Assert.That(caseWrapper.Records.Count, Is.EqualTo(2));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Exactly(2));
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsTask_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			await recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing");

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Never);
		}
	}
}