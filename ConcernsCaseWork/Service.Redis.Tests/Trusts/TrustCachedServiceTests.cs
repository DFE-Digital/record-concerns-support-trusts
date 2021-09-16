using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Trusts;
using Service.TRAMS.Trusts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustCachedServiceTests
	{
		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrusts_CacheIsNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustCachedService>>();

			var expectedTrust = TrustFactory.BuildTrustDetailsDto();
			
			mockTrustService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrust);
			
			var trustCachedService = new TrustCachedService(mockCacheProvider.Object, mockTrustService.Object, mockLogger.Object);
			
			// act
			var actualTrust = await trustCachedService.GetTrustByUkPrn("trust-ukprn");

			// assert
			Assert.That(actualTrust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
			Assert.That(actualTrust.GiasData, Is.Not.Null);
			Assert.That(actualTrust.GiasData.GroupId, Is.EqualTo(expectedTrust.GiasData.GroupId));
			Assert.That(actualTrust.GiasData.GroupName, Is.EqualTo(expectedTrust.GiasData.GroupName));
			Assert.That(actualTrust.GiasData.UkPrn, Is.EqualTo(expectedTrust.GiasData.UkPrn));
			Assert.That(actualTrust.GiasData.CompaniesHouseNumber, Is.EqualTo(expectedTrust.GiasData.CompaniesHouseNumber));
			Assert.That(actualTrust.GiasData.GroupContactAddress, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress));
			Assert.That(actualTrust.GiasData.GroupTypeCode, Is.EqualTo(expectedTrust.GiasData.GroupTypeCode));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IDictionary<string, TrustDetailsDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IDictionary<string, TrustDetailsDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockTrustService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrusts_CacheIsNotNull()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustCachedService>>();

			var expectedTrust = TrustFactory.BuildTrustDetailsDto();
			var trusts = new Dictionary<string, TrustDetailsDto>{ { "trust-ukprn", expectedTrust } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<IDictionary<string, TrustDetailsDto>>(It.IsAny<string>())).
				ReturnsAsync(trusts);
			
			var trustCachedService = new TrustCachedService(mockCacheProvider.Object, mockTrustService.Object, mockLogger.Object);
			
			// act
			var actualTrust = await trustCachedService.GetTrustByUkPrn("trust-ukprn");

			// assert
			Assert.That(actualTrust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
			Assert.That(actualTrust.GiasData, Is.Not.Null);
			Assert.That(actualTrust.GiasData.GroupId, Is.EqualTo(expectedTrust.GiasData.GroupId));
			Assert.That(actualTrust.GiasData.GroupName, Is.EqualTo(expectedTrust.GiasData.GroupName));
			Assert.That(actualTrust.GiasData.UkPrn, Is.EqualTo(expectedTrust.GiasData.UkPrn));
			Assert.That(actualTrust.GiasData.CompaniesHouseNumber, Is.EqualTo(expectedTrust.GiasData.CompaniesHouseNumber));
			Assert.That(actualTrust.GiasData.GroupContactAddress, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress));
			Assert.That(actualTrust.GiasData.GroupTypeCode, Is.EqualTo(expectedTrust.GiasData.GroupTypeCode));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IDictionary<string, TrustDetailsDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IDictionary<string, TrustDetailsDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockTrustService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}
		
		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrusts_CacheIsNotNull_KeyNotFound()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustCachedService>>();

			var expectedTrust = TrustFactory.BuildTrustDetailsDto();
			var trusts = new Dictionary<string, TrustDetailsDto>{ { "trust-ukprn-1", expectedTrust } };
			
			mockCacheProvider.Setup(c => c.GetFromCache<IDictionary<string, TrustDetailsDto>>(It.IsAny<string>())).
				ReturnsAsync(trusts);
			mockTrustService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrust);
			
			var trustCachedService = new TrustCachedService(mockCacheProvider.Object, mockTrustService.Object, mockLogger.Object);
			
			// act
			var actualTrust = await trustCachedService.GetTrustByUkPrn("trust-ukprn");

			// assert
			Assert.That(actualTrust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
			Assert.That(actualTrust.GiasData, Is.Not.Null);
			Assert.That(actualTrust.GiasData.GroupId, Is.EqualTo(expectedTrust.GiasData.GroupId));
			Assert.That(actualTrust.GiasData.GroupName, Is.EqualTo(expectedTrust.GiasData.GroupName));
			Assert.That(actualTrust.GiasData.UkPrn, Is.EqualTo(expectedTrust.GiasData.UkPrn));
			Assert.That(actualTrust.GiasData.CompaniesHouseNumber, Is.EqualTo(expectedTrust.GiasData.CompaniesHouseNumber));
			Assert.That(actualTrust.GiasData.GroupContactAddress, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress));
			Assert.That(actualTrust.GiasData.GroupTypeCode, Is.EqualTo(expectedTrust.GiasData.GroupTypeCode));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IDictionary<string, TrustDetailsDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IDictionary<string, TrustDetailsDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockTrustService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
	}
}