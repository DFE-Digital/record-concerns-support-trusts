using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Cases;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Sequence;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseHistoryCachedServiceTests
	{
		[Test]
		public async Task WhenPostCaseHistory_CacheIsNull_CreateCaseWrapper()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockLogger = new Mock<ILogger<CaseHistoryCachedService>>();
			var mockSequence = new Mock<ISequenceCachedService>();
			
			var caseHistoryCachedService = new CaseHistoryCachedService(mockCacheProvider.Object, mockCaseHistoryService.Object,
				mockCaseSearchService.Object, mockLogger.Object, mockSequence.Object);
			
			// act
			await caseHistoryCachedService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto(), "caseworker");
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostCaseHistory_CacheContainsKey_Add_CaseHistory()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockLogger = new Mock<ILogger<CaseHistoryCachedService>>();
			var mockSequence = new Mock<ISequenceCachedService>();
			
			var caseStateModel = new UserState("testing")
			{
				TrustUkPrn = "999999",
				CasesDetails = { { 1, new CaseWrapper() } }
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(caseStateModel);
			
			var caseHistoryCachedService = new CaseHistoryCachedService(mockCacheProvider.Object, mockCaseHistoryService.Object,
				mockCaseSearchService.Object, mockLogger.Object, mockSequence.Object);
			
			// act
			await caseHistoryCachedService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto(), "caseworker");
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostCaseHistory_CacheDoesntContainKey_CreateCaseWrapper_Add_CaseHistory()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockLogger = new Mock<ILogger<CaseHistoryCachedService>>();
			var mockSequence = new Mock<ISequenceCachedService>();
			
			var caseStateModel = new UserState("testing")
			{
				TrustUkPrn = "999999",
				CasesDetails = { { 1, new CaseWrapper() } }
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(caseStateModel);
			
			var caseHistoryCachedService = new CaseHistoryCachedService(mockCacheProvider.Object, mockCaseHistoryService.Object,
				mockCaseSearchService.Object, mockLogger.Object, mockSequence.Object);
			
			// act
			await caseHistoryCachedService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto(99), "caseworker");
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}

		[Test]
		public async Task WhenGetCasesHistory_CacheContainsKey_ReturnsListCaseHistory()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockLogger = new Mock<ILogger<CaseHistoryCachedService>>();
			var mockSequence = new Mock<ISequenceCachedService>();
			
			var caseStateModel = new UserState("testing")
			{
				TrustUkPrn = "999999",
				CasesDetails = { { 1, new CaseWrapper{ CasesHistoryDto = CaseFactory.BuildListCasesHistoryDto() } } },
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(caseStateModel);
			
			var caseHistoryCachedService = new CaseHistoryCachedService(mockCacheProvider.Object, mockCaseHistoryService.Object,
				mockCaseSearchService.Object, mockLogger.Object, mockSequence.Object);
			
			// act
			var casesHistoryDto = await caseHistoryCachedService.GetCasesHistory(CaseFactory.BuildCaseSearch(), "caseworker");
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			
			Assert.That(casesHistoryDto, Is.Not.Null);
			Assert.That(casesHistoryDto.Count, Is.EqualTo(1));
		}
		
		[Test]
		public async Task WhenGetCasesHistory_CacheDoesntContainsKey_ReturnsListCaseHistory()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockLogger = new Mock<ILogger<CaseHistoryCachedService>>();
			var mockSequence = new Mock<ISequenceCachedService>();

			var expectedCasesHistoryDto = CaseFactory.BuildListCasesHistoryDto();

			mockCaseSearchService.Setup(c => c.GetCasesHistoryByCaseSearch(It.IsAny<CaseSearch>()))
				.ReturnsAsync(expectedCasesHistoryDto);
			
			var caseHistoryCachedService = new CaseHistoryCachedService(mockCacheProvider.Object, mockCaseHistoryService.Object,
				mockCaseSearchService.Object, mockLogger.Object, mockSequence.Object);
			
			// act
			var actualCasesHistoryDto = await caseHistoryCachedService.GetCasesHistory(CaseFactory.BuildCaseSearch(), "caseworker");
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			
			Assert.That(actualCasesHistoryDto, Is.Not.Null);
			Assert.That(actualCasesHistoryDto.Count, Is.EqualTo(0));
		}
	}
}