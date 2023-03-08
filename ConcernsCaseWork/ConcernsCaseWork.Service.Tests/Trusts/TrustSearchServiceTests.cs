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

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10, TrustsPerPage = expectedTrusts.Count });
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>(), It.IsAny<int>()))
				.ReturnsAsync(new TrustSearchResponseDto { Trusts = expectedTrusts, NumberOfMatches = expectedTrusts.Count })
				.ReturnsAsync(new TrustSearchResponseDto { Trusts = emptyList, NumberOfMatches = 0 });

			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var results = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.EqualTo(expectedTrusts.Count));

			foreach (var trust in results.Trusts)
			{
				foreach (var expectedTrust in expectedTrusts)
				{
					Assert.That(trust.Urn, Is.EqualTo(expectedTrust.Urn));
					Assert.That(trust.GroupName, Is.EqualTo(expectedTrust.GroupName));
					Assert.That(trust.UkPrn, Is.EqualTo(expectedTrust.UkPrn));
					Assert.That(trust.CompaniesHouseNumber, Is.EqualTo(expectedTrust.CompaniesHouseNumber));
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
			var expectedResponseDto = new TrustSearchResponseDto() { Trusts = expectedTrusts, NumberOfMatches = 10 };

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10, TrustsPerPage = expectedTrusts.Count });
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>(), expectedTrusts.Count))
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto)
				.ReturnsAsync(expectedResponseDto);

			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var results = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.EqualTo(expectedTrusts.Count * 10));
		}

		[Test]
		public async Task WhenGetTrustsBySearchCriteria_AndLimitedByMaxPages_AndSomeResponsesAreNull_ReturnsTrustsFromTrams()
		{
			// arrange
			var mockTrustService = new Mock<ITrustService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<TrustSearchService>>();

			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			var expectedApiWrapper = new TrustSearchResponseDto { Trusts = expectedTrusts, NumberOfMatches = 10 };
			var expectedEmptyApiWrapper = new TrustSearchResponseDto { Trusts = null, NumberOfMatches = 0 };


			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10 });
			mockTrustService.SetupSequence(t => t.GetTrustsByPagination(It.IsAny<TrustSearch>(), It.IsAny<int>()))
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedApiWrapper)
				.ReturnsAsync(expectedEmptyApiWrapper);

			var trustSearchService = new TrustSearchService(mockTrustService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var results = await trustSearchService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.EqualTo(expectedTrusts.Count * 2));
		}
	}
}