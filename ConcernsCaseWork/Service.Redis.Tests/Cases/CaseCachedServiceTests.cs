using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseCachedServiceTests
	{
		[Test]
		public async Task WhenCreateCaseDataCache_IsSuccessful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var casesCachedService = new CachedService(mockCacheProvider.Object);
			var caseStateModel = new UserState
			{
				TrustUkPrn = "999999"
			};
			
			mockCacheProvider.Setup(c => c.CacheTimeToLive()).Returns(24);
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(caseStateModel));

			// act
			await casesCachedService.StoreData("username", caseStateModel);
			var cachedCaseStateData = await casesCachedService.GetData<UserState>("username");

			// assert
			Assert.That(cachedCaseStateData, Is.Not.Null);
			Assert.That(cachedCaseStateData, Is.InstanceOf<UserState>());
			Assert.That(cachedCaseStateData.TrustUkPrn, Is.EqualTo(caseStateModel.TrustUkPrn));
		}

		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCasesDto_FromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var caseStateModel = new UserState
			{
				TrustUkPrn = "999999",
				CasesDetails = { { 1, new CaseWrapper{ CaseDto = CaseFactory.BuildCaseDto() } } }
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(caseStateModel));

			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var casesDto = await caseCachedService.GetCasesByCaseworkerAndStatus("testing", 1);
			
			// assert
			Assert.That(casesDto, Is.Not.Null);
			Assert.That(casesDto.Count, Is.EqualTo(1));
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockCaseService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCasesDto_FromTramsApi()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult<UserState>(null));
			mockCaseService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(expectedCasesDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var casesDto = await caseCachedService.GetCasesByCaseworkerAndStatus("testing", 1);
			
			// assert
			Assert.That(casesDto, Is.Not.Null);
			Assert.That(casesDto.Count, Is.EqualTo(expectedCasesDto.Count));
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenGetCaseByUrn_ReturnsCasesDto_FromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var caseStateModel = new UserState
			{
				TrustUkPrn = "999999",
				CasesDetails = { { 1, new CaseWrapper{ CaseDto = CaseFactory.BuildCaseDto() } } }
			};
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(caseStateModel));

			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var casesDto = await caseCachedService.GetCaseByUrn("testing", 1);
			
			// assert
			Assert.That(casesDto, Is.Not.Null);
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockCaseService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenGetCaseByUrn_ReturnsCasesDto_FromTramsApi()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCaseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult<UserState>(null));
			mockCaseService.Setup(c => c.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(expectedCaseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var casesDto = await caseCachedService.GetCaseByUrn("testing", 1);
			
			// assert
			Assert.That(casesDto, Is.Not.Null);
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostCase_ReturnsCasesDto_StoresInCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCreateCaseDto = CaseFactory.BuildCreateCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult<UserState>(null));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var caseDto = await caseCachedService.PostCase(expectedCreateCaseDto);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCreateCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCreateCaseDto.Issue));
			Assert.That(caseDto.Status, Is.EqualTo(expectedCreateCaseDto.Status));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCreateCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCreateCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCreateCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCreateCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCreateCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCreateCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCreateCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCreateCaseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCreateCaseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCreateCaseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCreateCaseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCreateCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCreateCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCreateCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCreateCaseDto.TrustUkPrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPostCase_ReturnsCasesDto_StoresAddsToCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCreateCaseDto = CaseFactory.BuildCreateCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(new UserState()));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var caseDto = await caseCachedService.PostCase(expectedCreateCaseDto);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCreateCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCreateCaseDto.Issue));
			Assert.That(caseDto.Status, Is.EqualTo(expectedCreateCaseDto.Status));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCreateCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCreateCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCreateCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCreateCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCreateCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCreateCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCreateCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCreateCaseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCreateCaseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCreateCaseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCreateCaseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCreateCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCreateCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCreateCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCreateCaseDto.TrustUkPrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_StoresInCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult<UserState>(null));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(CaseFactory.BuildCaseDto());
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_StoresAddsToCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(new UserState()));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(CaseFactory.BuildCaseDto());
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_UpdatesCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var userState = new UserState { CasesDetails = { { 1, new CaseWrapper { CaseDto = CaseFactory.BuildCaseDto() } } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(userState));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(CaseFactory.BuildCaseDto());
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenIsCasePrimary_ReturnsTrue()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var userState = new UserState { CasesDetails = { { 1, new CaseWrapper() } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(userState));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var isPrimary = await caseCachedService.IsCasePrimary("testing", 1);
			
			// assert
			Assert.That(isPrimary, Is.True);
		}
		
		[Test]
		public async Task WhenIsCasePrimary_ReturnsTrue_DefaultReturn()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var userState = new UserState { CasesDetails = { { 1, new CaseWrapper() } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(userState));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var isPrimary = await caseCachedService.IsCasePrimary("testing", 2);
			
			// assert
			Assert.That(isPrimary, Is.True);
		}
		
		[Test]
		public async Task WhenIsCasePrimary_ReturnsFalse()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var userState = new UserState { CasesDetails = { { 1, new CaseWrapper{ Records = { { 1, new RecordWrapper() } } } } } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult(userState));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var isPrimary = await caseCachedService.IsCasePrimary("testing", 1);
			
			// assert
			Assert.That(isPrimary, Is.False);
		}
		
		[Test]
		public async Task WhenIsCasePrimary_ReturnsFalse_UserState_IsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				Returns(Task.FromResult<UserState>(null));
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var isPrimary = await caseCachedService.IsCasePrimary("testing", 1);
			
			// assert
			Assert.That(isPrimary, Is.True);
		}
	}
}