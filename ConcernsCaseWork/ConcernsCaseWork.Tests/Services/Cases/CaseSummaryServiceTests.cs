using AutoFixture;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Services.Cases;
using FluentAssertions;
using Microsoft.Extensions.Azure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases;

[Parallelizable(ParallelScope.All)]
public class CaseSummaryServiceTests
{
	private readonly IFixture _fixture = new Fixture();

	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = new List<ActiveCaseSummaryDto>();
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Should().BeEmpty();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCases_ReturnsList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = BuildListActiveCaseSummaryDtos();
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
		
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"), 
			new(DateTime.Now, null, "some name 2"), 
			new(DateTime.Now, null, "some name 3"), 
			new(DateTime.Now, null, "some name 4")
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeTrue();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCasesWith3OFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(emptyActionsAndDecisions: true);
		data.Decisions = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeFalse();
	}

	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsNoMoreThan3ActionsAndDecisionsInCreatedDateOrder()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(emptyActionsAndDecisions: true);
		data.Decisions = new ActiveCaseSummaryDto.ActionDecisionSummaryDto[]
		{
			new(DateTime.Now.AddDays(-3), null, "1"),
			new(DateTime.Now, null, "Not returned"),
			new(DateTime.Now.AddDays(-2), null, "2"), 
			new(DateTime.Now.AddDays(-1), null, "3")
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Single().ActiveActionsAndDecisions.Length.Should().Be(3);
		result.Single().ActiveActionsAndDecisions[0].Should().Be("1");
		result.Single().ActiveActionsAndDecisions[1].Should().Be("2");
		result.Single().ActiveActionsAndDecisions[2].Should().Be("3");
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_ReturnsCasesSortedByCreatedDateDescending()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildListActiveCaseSummaryDtos();

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(data.Count);
		var sortedData = data.OrderByDescending(d => d.CreatedAt);
		result.Select(r => r.CreatedAt).Should().ContainInConsecutiveOrder(sortedData.Select(r => r.CreatedAt.ToDayMonthYear()));
	}
		
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_ReturnsCasesWithConcernsSortedByRagRating()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto();
		data.ActiveConcerns = new List<ActiveCaseSummaryDto.ConcernSummaryDto> { 
			new("4", new RatingDto("Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("2", new RatingDto("Red", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("1", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("5", new RatingDto("Amber-Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("6", new RatingDto("Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("3", new RatingDto("Red-Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now)
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInAscendingOrder();
	}

	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirstWhenRagRatingsTheSame()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto();
		data.ActiveConcerns = new List<ActiveCaseSummaryDto.ConcernSummaryDto>
		{
			new("4", new RatingDto("Amber", DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now, 1), DateTime.Now),
			new("3", new RatingDto("Amber", DateTimeOffset.Now.AddDays(-2), DateTimeOffset.Now, 1), DateTime.Now),
			new("2", new RatingDto("Red-Plus", DateTimeOffset.Now.AddDays(-3), DateTimeOffset.Now, 1), DateTime.Now),
			new("1", new RatingDto("Red-Plus", DateTimeOffset.Now.AddDays(-4), DateTimeOffset.Now, 1), DateTime.Now),
			new("6", new RatingDto("Green", DateTimeOffset.Now.AddDays(-5), DateTimeOffset.Now, 1), DateTime.Now),
			new("5", new RatingDto("Green", DateTimeOffset.Now.AddDays(-6), DateTimeOffset.Now, 1), DateTime.Now)
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(c => int.Parse(c)).Should().BeInAscendingOrder();
	}

	private List<ActiveCaseSummaryDto> BuildListActiveCaseSummaryDtos()
		=> _fixture.CreateMany<ActiveCaseSummaryDto>().ToList();

	private ActiveCaseSummaryDto BuildActiveCaseSummaryDto(bool emptyActionsAndDecisions = false)
	{
		var dto = _fixture.Create<ActiveCaseSummaryDto>();
		
		if (emptyActionsAndDecisions)
		{
			dto.Decisions = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
			dto.NoticesToImprove = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
			dto.NtisUnderConsideration = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
			dto.NtiWarningLetters = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
			dto.SrmaCases = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
			dto.FinancialPlanCases = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		}

		return dto;
	}
}