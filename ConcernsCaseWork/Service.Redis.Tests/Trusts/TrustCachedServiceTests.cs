using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Trusts;
using Service.TRAMS.Models;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustCachedServiceTests
	{
		[Test]
		public async Task WhenGetTrustsCached_ReturnsTrustsFromTrams()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<TrustCachedService>>();

			var expectedTrusts = TrustDtoFactory.CreateListTrustDto();
			IList<TrustDto> emptyList = Array.Empty<TrustDto>();

			mockCacheProvider.Setup(c => c.CacheTimeToLive()).Returns(120);
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<int>()))
				.Returns(Task.FromResult(expectedTrusts))
				.Returns(Task.FromResult(emptyList));
			
			var trustCachedService = new TrustCachedService(mockTrustService.Object, mockCacheProvider.Object, mockLogger.Object);

			// act
			var trusts = await trustCachedService.GetTrustsCached();

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(expectedTrusts.Count));

			foreach (var trust in trusts)
			{
				foreach (var expectedTrust in expectedTrusts)
				{
					Assert.That(trust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
					Assert.That(trust.Urn, Is.EqualTo(expectedTrust.Urn));
					Assert.That(trust.GroupName, Is.EqualTo(expectedTrust.GroupName));
					Assert.That(trust.UkPrn, Is.EqualTo(expectedTrust.UkPrn));
					Assert.That(trust.CompaniesHouseNumber, Is.EqualTo(expectedTrust.CompaniesHouseNumber));

					foreach (var establishment in trust.Establishments)
					{
						foreach (var expectedEstablishment in expectedTrust.Establishments)
						{
							Assert.That(establishment.Name, Is.EqualTo(expectedEstablishment.Name));
							Assert.That(establishment.Urn, Is.EqualTo(expectedEstablishment.Urn));
							Assert.That(establishment.UkPrn, Is.EqualTo(expectedEstablishment.UkPrn));
						}
					}
				}
			}
		}

		[Test]
		public async Task WhenGetTrustsCached_ReturnsTrustsFromCache()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockLogger = new Mock<ILogger<TrustCachedService>>();
	
			var expectedTrusts = TrustDtoFactory.CreateListTrustDto();

			mockCacheProvider.Setup(c => c.GetFromCache<List<TrustDto>>(It.IsAny<string>()))
				.Returns(Task.FromResult(expectedTrusts.ToList()));
			
			var trustCachedService = new TrustCachedService(mockTrustService.Object, mockCacheProvider.Object, mockLogger.Object);
			
			// act
			var trusts = await trustCachedService.GetTrustsCached();
			
			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(expectedTrusts.Count));
			
			foreach (var trust in trusts)
			{
				foreach (var expectedTrust in expectedTrusts)
				{
					Assert.That(trust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
					Assert.That(trust.Urn, Is.EqualTo(expectedTrust.Urn));
					Assert.That(trust.GroupName, Is.EqualTo(expectedTrust.GroupName));
					Assert.That(trust.UkPrn, Is.EqualTo(expectedTrust.UkPrn));
					Assert.That(trust.CompaniesHouseNumber, Is.EqualTo(expectedTrust.CompaniesHouseNumber));

					foreach (var establishment in trust.Establishments)
					{
						foreach (var expectedEstablishment in expectedTrust.Establishments)
						{
							Assert.That(establishment.Name, Is.EqualTo(expectedEstablishment.Name));
							Assert.That(establishment.Urn, Is.EqualTo(expectedEstablishment.Urn));
							Assert.That(establishment.UkPrn, Is.EqualTo(expectedEstablishment.UkPrn));
						}
					}
				}
			}
		}
	}
}