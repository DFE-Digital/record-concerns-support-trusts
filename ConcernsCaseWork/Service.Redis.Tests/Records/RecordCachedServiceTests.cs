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

			var expectedCreateRecordDto = RecordFactory.BuildCreateRecordDto();
			var newRecordDto = RecordFactory.BuildRecordDto();

			mockRecordService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync(newRecordDto);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecord = await recordRecordCachedService.PostRecordByCaseUrn(expectedCreateRecordDto, "testing");

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(newRecordDto.Name));
			Assert.That(actualRecord.CreatedAt, Is.Not.Null);
			Assert.That(actualRecord.Description, Is.EqualTo(newRecordDto.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(newRecordDto.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(newRecordDto.Reason));
			Assert.That(actualRecord.StatusUrn, Is.EqualTo(newRecordDto.StatusUrn));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(newRecordDto.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(newRecordDto.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(newRecordDto.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(newRecordDto.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(newRecordDto.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(newRecordDto.UpdatedAt));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecordDto_CacheIsNotNull_NewKey()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedCreateRecordDto = RecordFactory.BuildCreateRecordDto();
			var newRecordDto = RecordFactory.BuildRecordDto();

			mockRecordService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync(newRecordDto);
			
			var userState = new UserState();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecord = await recordRecordCachedService.PostRecordByCaseUrn(expectedCreateRecordDto, "testing");

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(newRecordDto.Name));
			Assert.That(actualRecord.CreatedAt, Is.Not.Null);
			Assert.That(actualRecord.Description, Is.EqualTo(newRecordDto.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(newRecordDto.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(newRecordDto.Reason));
			Assert.That(actualRecord.StatusUrn, Is.EqualTo(newRecordDto.StatusUrn));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(newRecordDto.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(newRecordDto.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(newRecordDto.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(newRecordDto.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(newRecordDto.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(newRecordDto.UpdatedAt));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecordDto_CacheIsNotNull_KeyExists()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedCreateRecordDto = RecordFactory.BuildCreateRecordDto();
			var newRecordDto = RecordFactory.BuildRecordDto();

			mockRecordService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync(newRecordDto);
			
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CasesDetails = { { 1, new CaseWrapper { CaseDto = CaseFactory.BuildCaseDto(), } } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			var actualRecord = await recordRecordCachedService.PostRecordByCaseUrn(expectedCreateRecordDto, "testing");

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(newRecordDto.Name));
			Assert.That(actualRecord.CreatedAt, Is.Not.Null);
			Assert.That(actualRecord.Description, Is.EqualTo(newRecordDto.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(newRecordDto.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(newRecordDto.Reason));
			Assert.That(actualRecord.StatusUrn, Is.EqualTo(newRecordDto.StatusUrn));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(newRecordDto.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(newRecordDto.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(newRecordDto.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(newRecordDto.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(newRecordDto.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(newRecordDto.UpdatedAt));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPostRecordByCaseUrn_And_ResponseIsNull_ThrowsException()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedCreateRecordDto = RecordFactory.BuildCreateRecordDto();
			
			mockRecordService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync((RecordDto)null);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			Assert.ThrowsAsync<ApplicationException>(() => recordRecordCachedService.PostRecordByCaseUrn(expectedCreateRecordDto, "testing"));

			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Never);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => c.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Once);
		}		
		
		[Test]
		public async Task WhenGetRecordsByCaseUrn_ReturnsRecordsDto_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var expectedRecordsDto = RecordFactory.BuildListRecordDto();
			
			mockRecordService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(expectedRecordsDto);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);

			var caseDto = CaseFactory.BuildCaseDto();

			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(expectedRecordsDto.Count));

			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public void WhenGetRecordsByCaseUrn_CacheIsNull_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();
			
			mockRecordService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync((List<RecordDto>)null);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);

			var caseDto = CaseFactory.BuildCaseDto();

			// act
			Assert.ThrowsAsync<ApplicationException>(() => recordRecordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn));

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Once);
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

			var caseDto = CaseFactory.BuildCaseDto();

			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(1));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => c.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Never);
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

			var caseDto = CaseFactory.BuildCaseDto();

			// act
			var actualRecords = await recordRecordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);

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

			var recordDto = RecordFactory.BuildRecordDto();
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
								RecordDto = recordDto, 
								RecordsRatingHistory = new List<RecordRatingHistoryDto>
								{
									new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1)
								}
							} }
						}
					} }
				}
			};

			mockRecordService.Setup(r => r.PatchRecordByUrn(It.IsAny<RecordDto>())).ReturnsAsync(recordDto);
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			await recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing");

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsTask_CacheIsNotNull_KeyMissing()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();

			var recordDto = RecordFactory.BuildRecordDto();
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
								RecordDto = recordDto, 
								RecordsRatingHistory = new List<RecordRatingHistoryDto>
								{
									new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1)
								}
							} }
						}
					} }
				}
			}; 
			
			mockRecordService.Setup(r => r.PatchRecordByUrn(It.IsAny<RecordDto>())).ReturnsAsync(recordDto);
			
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
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsTask_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();
			
			var recordDto = RecordFactory.BuildRecordDto();
			mockRecordService.Setup(r => r.PatchRecordByUrn(It.IsAny<RecordDto>())).ReturnsAsync(recordDto);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			await recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing");

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchRecordByUrn_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRecordService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordCachedService>>();
			
			mockRecordService.Setup(r => r.PatchRecordByUrn(It.IsAny<RecordDto>())).ReturnsAsync((RecordDto)null);
			
			var recordRecordCachedService = new RecordCachedService(
				mockCacheProvider.Object, mockRecordService.Object, mockLogger.Object);
			
			// act
			Assert.ThrowsAsync<ApplicationException>(() => recordRecordCachedService.PatchRecordByUrn(RecordFactory.BuildRecordDto(), "testing"));

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Never);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRecordService.Verify(c => c.PatchRecordByUrn(It.IsAny<RecordDto>()), Times.Once);
		}
	}
}