﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ConcernsCaseWork.Service.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseSearchServiceTests
	{
		public static readonly Fixture _fixture = new();

		[Test]
		public async Task WhenGetCasesByPageSearch_ReturnsCasesFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			IList<CaseDto> emptyList = Array.Empty<CaseDto>();

			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(c => c.GetCases(It.IsAny<PageSearch>()))
				.ReturnsAsync(new ApiListWrapper<CaseDto>(expectedCasesDto, null))
				.ReturnsAsync(new ApiListWrapper<CaseDto>(emptyList, null));
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesDto = await caseSearchService.GetCasesByPageSearch(CaseFactory.BuildPageSearch());

			// assert
			Assert.That(actualCasesDto, Is.Not.Null);
			Assert.That(actualCasesDto.Count, Is.EqualTo(expectedCasesDto.Count));

			foreach (var caseDto in actualCasesDto)
			{
				foreach (var expectedCase in expectedCasesDto.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Issue, Is.EqualTo(expectedCase.Issue));
					Assert.That(caseDto.StatusId, Is.EqualTo(expectedCase.StatusId));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
					Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
					Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
					Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
					Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
					Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCase.NextSteps));
					Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCase.CaseAim));
					Assert.That(caseDto.CaseHistory, Is.EqualTo(expectedCase.CaseHistory));
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
		public async Task WhenGetCasesByPageSearch_AndLimitedByMaxPages_ReturnsCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var pagination = _fixture.Create<Pagination>();
			pagination.Page = 1;
			pagination.RecordCount = 200;

			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, pagination);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(c => c.GetCases(It.IsAny<PageSearch>()))
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
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesDto = await caseSearchService.GetCasesByPageSearch(CaseFactory.BuildPageSearch());

			// assert
			Assert.That(actualCasesDto, Is.Not.Null);
			Assert.That(actualCasesDto.Count, Is.EqualTo(expectedCasesDto.Count * 11));
		}
		
		[Test]
		public async Task WhenGetCasesByPageSearch_AndLimitedByPagination_ReturnsFirstBatchCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, null);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(c => c.GetCases(It.IsAny<PageSearch>()))
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
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesDto = await caseSearchService.GetCasesByPageSearch(CaseFactory.BuildPageSearch());

			// assert
			Assert.That(actualCasesDto, Is.Not.Null);
			Assert.That(actualCasesDto.Count, Is.EqualTo(expectedCasesDto.Count));
		}		
		
		[Test]
		public async Task WhenGetCasesByPageSearch_AndResponseIsNullOrDataIsNull_ReturnsEmptyCaseTrustsFromTrams()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockIOptionsTrustSearch = new Mock<IOptions<TrustSearchOptions>>();
			var mockLogger = new Mock<ILogger<CaseSearchService>>();

			var expectedCasesDto = CaseFactory.BuildListCaseDto();
			var expectedApiWrapperCasesDto = new ApiListWrapper<CaseDto>(expectedCasesDto, null);
			var nullApiWrapperCasesDto = new ApiListWrapper<CaseDto>(null, null);
			
			mockIOptionsTrustSearch.Setup(o => o.Value).Returns(new TrustSearchOptions { TrustsLimitByPage = 10});
			mockCaseService.SetupSequence(c => c.GetCases(It.IsAny<PageSearch>()))
				.ReturnsAsync((ApiListWrapper<CaseDto>)null)
				.ReturnsAsync(expectedApiWrapperCasesDto)
				.ReturnsAsync(nullApiWrapperCasesDto);
			
			var caseSearchService = new CaseSearchService(mockCaseService.Object, mockIOptionsTrustSearch.Object, mockLogger.Object);

			// act
			var actualCasesDto = await caseSearchService.GetCasesByPageSearch(CaseFactory.BuildPageSearch());

			// assert
			Assert.That(actualCasesDto, Is.Not.Null);
			Assert.That(actualCasesDto.Count, Is.EqualTo(0));
		}		
	}
}