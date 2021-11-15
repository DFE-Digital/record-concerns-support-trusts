using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using Service.TRAMS.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseSearchServiceTests
	{
		[Test]
		public async Task WhenGetCasesBySearchCriteria_ReturnsCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			IList<CaseDto> emptyList = Array.Empty<CaseDto>();

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(t => t.GetCasesByTrustUkPrn(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(new ApiListWrapper<CaseDto>(expectedCasesDto, null))
				.ReturnsAsync(new ApiListWrapper<CaseDto>(emptyList, null));
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesDto = await caseSearchService.GetCasesByCaseTrustSearch(CaseFactory.BuildCaseTrustSearch());

			// assert
			Assert.That(actualCasesDto, Is.Not.Null);
			Assert.That(actualCasesDto.Count, Is.EqualTo(expectedCasesDto.Count));

			foreach (var caseDto in actualCasesDto)
			{
				foreach (var expectedCase in expectedCasesDto.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Issue, Is.EqualTo(expectedCase.Issue));
					Assert.That(caseDto.StatusUrn, Is.EqualTo(expectedCase.StatusUrn));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
					Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
					Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
					Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
					Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
					Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCase.NextSteps));
					Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCase.CaseAim));
					Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
					Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
					Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
					Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
					Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
					Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
				}
			}
		}

		[Test]
		public async Task WhenGetCasesBySearchCriteria_AndLimitedByMaxPages_ReturnsCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, new ApiListWrapper<CaseDto>.Pagination(1, 200, string.Empty));
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(t => t.GetCasesByTrustUkPrn(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCaseTrusts = await caseSearchService.GetCasesByCaseTrustSearch(CaseFactory.BuildCaseTrustSearch());

			// assert
			Assert.That(actualCaseTrusts, Is.Not.Null);
			Assert.That(actualCaseTrusts.Count, Is.EqualTo(expectedCasesDto.Count * 11));
		}
		
		[Test]
		public async Task WhenGetCasesBySearchCriteria_AndLimitedByPagination_ReturnsFirstBatchCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, null);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(t => t.GetCasesByTrustUkPrn(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(expectedApiWrapperCasesDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCaseTrusts = await caseSearchService.GetCasesByCaseTrustSearch(CaseFactory.BuildCaseTrustSearch());

			// assert
			Assert.That(actualCaseTrusts, Is.Not.Null);
			Assert.That(actualCaseTrusts.Count, Is.EqualTo(expectedCasesDto.Count));
		}
		
		[Test]
		public async Task WhenGetCasesBySearchCriteria_AndResponseIsNullOrDataIsNull_ReturnsEmptyCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, null);
			var nullApiWrapperCasesDto = new ApiListWrapper<CaseDto>(null, null);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(t => t.GetCasesByTrustUkPrn(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync((ApiListWrapper<CaseDto>)null)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(nullApiWrapperCasesDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCaseTrusts = await caseSearchService.GetCasesByCaseTrustSearch(CaseFactory.BuildCaseTrustSearch());

			// assert
			Assert.That(actualCaseTrusts, Is.Not.Null);
			Assert.That(actualCaseTrusts.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task WhenGetCasesHistoryByCaseSearch_ReturnsCasesHistoryFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesHistoryDto = CaseFactory.BuildListCasesHistoryDto();
			IList<CaseHistoryDto> emptyList = Array.Empty<CaseHistoryDto>();

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseHistoryService.SetupSequence(c => c.GetCasesHistory(It.IsAny<CaseSearch>()))
				.ReturnsAsync(new ApiListWrapper<CaseHistoryDto>(expectedCasesHistoryDto, null))
				.ReturnsAsync(new ApiListWrapper<CaseHistoryDto>(emptyList, null));
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesHistoryDto = await caseSearchService.GetCasesHistoryByCaseSearch(CaseFactory.BuildCaseSearch());

			// assert
			Assert.That(actualCasesHistoryDto, Is.Not.Null);
			Assert.That(actualCasesHistoryDto.Count, Is.EqualTo(expectedCasesHistoryDto.Count));
			
			foreach (var caseDto in actualCasesHistoryDto)
			{
				foreach (var expectedCase in expectedCasesHistoryDto.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Action, Is.EqualTo(expectedCase.Action));
					Assert.That(caseDto.Title, Is.EqualTo(expectedCase.Title));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.CaseUrn, Is.EqualTo(expectedCase.CaseUrn));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesHistoryByCaseSearch_AndLimitedByMaxPages_ReturnsCasesHistoryFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesHistoryDto = CaseFactory.BuildListCasesHistoryDto();
			var expectedApiWrapperCasesHistoryDto = new ApiListWrapper<CaseHistoryDto>(expectedCasesHistoryDto, new ApiListWrapper<CaseHistoryDto>.Pagination(1, 200, string.Empty));
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseHistoryService.SetupSequence(c => c.GetCasesHistory(It.IsAny<CaseSearch>()))
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesHistory = await caseSearchService.GetCasesHistoryByCaseSearch(CaseFactory.BuildCaseSearch());

			// assert
			Assert.That(actualCasesHistory, Is.Not.Null);
			Assert.That(actualCasesHistory.Count, Is.EqualTo(expectedCasesHistoryDto.Count * 11));
		}
		
		[Test]
		public async Task WhenGetCasesHistoryByCaseSearch_AndResponseIsNullOrDataIsNull_ReturnsEmptyCasesHistoryFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockCaseHistoryService = new Mock<ICaseHistoryService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesHistoryDto = CaseFactory.BuildListCasesHistoryDto();
			var expectedApiWrapperCasesHistoryDto = new ApiListWrapper<CaseHistoryDto>(expectedCasesHistoryDto, null);
			var nullApiWrapperCasesHistoryDto = new ApiListWrapper<CaseHistoryDto>(null, null);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseHistoryService.SetupSequence(c => c.GetCasesHistory(It.IsAny<CaseSearch>()))
				.ReturnsAsync((ApiListWrapper<CaseHistoryDto>)null)
				.ReturnsAsync(expectedApiWrapperCasesHistoryDto)
				.ReturnsAsync(nullApiWrapperCasesHistoryDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockCaseHistoryService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesHistory = await caseSearchService.GetCasesHistoryByCaseSearch(CaseFactory.BuildCaseSearch());

			// assert
			Assert.That(actualCasesHistory, Is.Not.Null);
			Assert.That(actualCasesHistory.Count, Is.EqualTo(0));
		}
	}
}