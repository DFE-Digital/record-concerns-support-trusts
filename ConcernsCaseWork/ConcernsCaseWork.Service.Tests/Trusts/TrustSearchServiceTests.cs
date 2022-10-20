using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Configuration;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ConcernsCaseWork.Service.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustSearchServiceTests
	{
		[Test]
		public async Task WhenGetTrustsBySearchCriteria_ReturnsTrustsFromTrams()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<TrustSearchService>>();

			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			IList<TrustSearchDto> emptyList = Array.Empty<TrustSearchDto>();

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>()))
				.ReturnsAsync(new ApiListWrapper<TrustSearchDto>(expectedTrusts, null))
				.ReturnsAsync(new ApiListWrapper<TrustSearchDto>(emptyList, null));
			
			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var trusts = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

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
		public async Task WhenGetTrustsBySearchCriteria_ReturnsTrustsFromTrams_LimitedByMaxPages()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<TrustSearchService>>();

			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			var expectedApiWrapper = new ApiListWrapper<TrustSearchDto>(
				expectedTrusts, 
				new ApiListWrapper<TrustSearchDto>.Pagination(1, 10, "next-page-url"));
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>()))
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper);
			
			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var trusts = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(expectedTrusts.Count * 11));
		}
		
		[Test]
		public async Task WhenGetTrustsBySearchCriteria_AndLimitedByMaxPages_AndSomeResponsesAreNull_ReturnsTrustsFromTrams()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<TrustSearchService>>();

			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			var expectedApiWrapper = new ApiListWrapper<TrustSearchDto>(
				expectedTrusts, 
				new ApiListWrapper<TrustSearchDto>.Pagination(1, 10, "next-page-url"));
			var expectedEmptyApiWrapper = new ApiListWrapper<TrustSearchDto>(
				null, 
				new ApiListWrapper<TrustSearchDto>.Pagination(1, 10, "next-page-url"));

			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>()))
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedEmptyApiWrapper);
			
			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var trusts = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(expectedTrusts.Count * 2));
		}
	}
}