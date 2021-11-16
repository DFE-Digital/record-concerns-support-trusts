using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
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
				ReturnsAsync(caseStateModel);

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
				ReturnsAsync(caseStateModel);

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
				ReturnsAsync((UserState)null);
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
			var expectedCaseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync((UserState)null);
			mockCaseService.Setup(c => c.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync(expectedCaseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var caseDto = await caseCachedService.PostCase(expectedCreateCaseDto);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCaseDto.Issue));
			Assert.That(caseDto.StatusUrn, Is.EqualTo(expectedCaseDto.StatusUrn));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCaseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCaseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCaseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCaseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PostCase(It.IsAny<CreateCaseDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPostCase_ReturnsCasesDto_StoresAddsToCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCreateCaseDto = CaseFactory.BuildCreateCaseDto();
			var expectedCaseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(new UserState());
			mockCaseService.Setup(c => c.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync(expectedCaseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var caseDto = await caseCachedService.PostCase(expectedCreateCaseDto);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCaseDto.Issue));
			Assert.That(caseDto.StatusUrn, Is.EqualTo(expectedCaseDto.StatusUrn));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCaseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCaseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCaseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCaseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PostCase(It.IsAny<CreateCaseDto>()), Times.Once);
		}

		[Test]
		public void WhenPostCase_ThrowsException_WhenApiCallReturnsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var expectedCreateCaseDto = CaseFactory.BuildCreateCaseDto();
			
			mockCaseService.Setup(c => c.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync((CaseDto)null);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act | assert
			Assert.ThrowsAsync<ApplicationException>(() => caseCachedService.PostCase(expectedCreateCaseDto));
			
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Never);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockCaseService.Verify(c => c.PostCase(It.IsAny<CreateCaseDto>()), Times.Once);
		}		
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_StoresInCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync((UserState)null);
			mockCaseService.Setup(c => c.PatchCaseByUrn(It.IsAny<CaseDto>())).ReturnsAsync(caseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(caseDto);
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_StoresAddsToCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(new UserState());
			mockCaseService.Setup(c => c.PatchCaseByUrn(It.IsAny<CaseDto>())).ReturnsAsync(caseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(caseDto);
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsTask_UpdatesCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseCachedService>>();

			var userState = new UserState { CasesDetails = { { 1, new CaseWrapper { CaseDto = CaseFactory.BuildCaseDto() } } } };
			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<UserState>(It.IsAny<string>())).
				ReturnsAsync(userState);
			mockCaseService.Setup(c => c.PatchCaseByUrn(It.IsAny<CaseDto>())).ReturnsAsync(caseDto);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			await caseCachedService.PatchCaseByUrn(caseDto);
			
			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<UserState>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCaseService.Verify(c => c.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
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
				ReturnsAsync(userState);
			
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
				ReturnsAsync(userState);
			
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
				ReturnsAsync(userState);
			
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
				ReturnsAsync((UserState)null);
			
			var caseCachedService = new CaseCachedService(mockCacheProvider.Object, mockCaseService.Object, mockLogger.Object);
			
			// act
			var isPrimary = await caseCachedService.IsCasePrimary("testing", 1);
			
			// assert
			Assert.That(isPrimary, Is.True);
		}
	}
}