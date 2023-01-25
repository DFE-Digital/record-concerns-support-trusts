using AutoFixture;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Services.Cases;
using FluentAssertions;
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

	#region Active by Caseworker
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(new List<ActiveCaseSummaryDto>());
		
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

		var data = BuildListActiveCaseSummaryDtos(userName);
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
	
	[Test]
	[TestCase("foo.bar@foobar.com", "Foo Bar")]
	[TestCase("someuser@foobar.com", "Someuser")]
	[TestCase("foo.bar", "foo.bar")]
	[TestCase("foo.bar@", "Foo Bar")]
	[TestCase("SOMEUSER", "SOMEUSER")]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCaseOwnerIsEmail_ReturnsListWithCaseWorkerNameFormatted(string userName, string expectedFormattedName)
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var data = BuildListActiveCaseSummaryDtos(userName);
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Should().HaveCount(data.Count);
		result.All(r => r.CreatedBy == expectedFormattedName).Should().BeTrue();
	}
		
	[Test]
	public async Task GetActiveCaseSummariesByCaseworker_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>
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
	public async Task GetActiveCaseSummariesByCaseworker_WhenCasesWith3OrFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.TrustFinancialForecasts = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

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
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new CaseSummaryDto.ActionDecisionSummaryDto[]
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
		var data = BuildActiveCaseSummaryDto(userName);
		data.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto> { 
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
	[TestCase("Amber")]
	[TestCase("Red")]
	[TestCase("Red-Amber")]
	[TestCase("Amber-Green")]
	[TestCase("Red-Plus")]
	[TestCase("Green")]
	public async Task GetActiveCaseSummariesByCaseworker_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirstWhenRagRatingsTheSame(string ragRating)
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName);
		var ratingDto = new RatingDto(ragRating, DateTimeOffset.Now, DateTimeOffset.Now, 1);
		data.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto>
		{
			new("4", ratingDto, DateTime.Now.AddDays(-4)),
			new("3", ratingDto, DateTime.Now.AddDays(-3)),
			new("2", ratingDto, DateTime.Now.AddDays(-2)),
			new("5", ratingDto, DateTime.Now.AddDays(-5)),
			new("1", ratingDto, DateTime.Now.AddDays(-1)), // newest should be last
			new("6", ratingDto, DateTime.Now.AddDays(-6)) // oldest should be first
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInDescendingOrder();
	}
	
	#endregion
	
	#region Active by Caseworker's team members
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(new List<ActiveCaseSummaryDto>());
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Should().BeEmpty();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_WhenCases_ReturnsList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = BuildListActiveCaseSummaryDtos();
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
		
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>
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
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeTrue();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_WhenCasesWith3OrFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeFalse();
	}

	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsNoMoreThan3ActionsAndDecisionsInCreatedDateOrder()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new CaseSummaryDto.ActionDecisionSummaryDto[]
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
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Single().ActiveActionsAndDecisions.Length.Should().Be(3);
		result.Single().ActiveActionsAndDecisions[0].Should().Be("1");
		result.Single().ActiveActionsAndDecisions[1].Should().Be("2");
		result.Single().ActiveActionsAndDecisions[2].Should().Be("3");
	}
	
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_ReturnsCasesSortedByCreatedDateDescending()
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
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Count.Should().Be(data.Count);
		var sortedData = data.OrderByDescending(d => d.CreatedAt);
		result.Select(r => r.CreatedAt).Should().ContainInConsecutiveOrder(sortedData.Select(r => r.CreatedAt.ToDayMonthYear()));
	}
		
	[Test]
	public async Task GetActiveCaseSummariesForTeamMembers_ReturnsCasesWithConcernsSortedByRagRating()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName);
		data.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto> { 
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
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInAscendingOrder();
	}

	[Test]
	[TestCase("Amber")]
	[TestCase("Red")]
	[TestCase("Red-Amber")]
	[TestCase("Amber-Green")]
	[TestCase("Red-Plus")]
	[TestCase("Green")]
	public async Task GetActiveCaseSummariesForTeamMembers_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirstWhenRagRatingsTheSame(string ragRating)
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data1 = BuildActiveCaseSummaryDto(userName);
		var ratingDto = new RatingDto(ragRating, DateTimeOffset.Now, DateTimeOffset.Now, 1);
		data1.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto>
		{
			new("4", ratingDto, DateTime.Now.AddDays(-4)),
			new("1", ratingDto, DateTime.Now.AddDays(-1)), // newest should be last
			new("3", ratingDto, DateTime.Now.AddDays(-3)),
			new("5", ratingDto, DateTime.Now.AddDays(-5)),
			new("2", ratingDto, DateTime.Now.AddDays(-2)),
			new("6", ratingDto, DateTime.Now.AddDays(-6)) // oldest should be first
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data1 });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesForTeamMembers(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInDescendingOrder();
	}
	
	#endregion
	
	#region Closed by Caseworker
	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		mockCaseSummaryService.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName)).ReturnsAsync(new List<ClosedCaseSummaryDto>());
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Should().BeEmpty();
	}
	
	
	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_WhenCases_ReturnsList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = BuildListClosedCaseSummaryDtos();
		mockCaseSummaryService.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
		
	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"), 
			new(DateTime.Now, null, "some name 2"), 
			new(DateTime.Now, null, "some name 3"), 
			new(DateTime.Now, null, "some name 4")
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeTrue();
	}
	
	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_WhenCasesWith3OrFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.TrustFinancialForecasts = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeFalse();
	}

	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsNoMoreThan3ActionsAndDecisionsInCreatedDateOrder()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new CaseSummaryDto.ActionDecisionSummaryDto[]
		{
			new(DateTime.Now.AddDays(-3), null, "1"),
			new(DateTime.Now, null, "Not returned"),
			new(DateTime.Now.AddDays(-2), null, "2"), 
			new(DateTime.Now.AddDays(-1), null, "3")
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Single().ClosedActionsAndDecisions.Length.Should().Be(3);
		result.Single().ClosedActionsAndDecisions[0].Should().Be("1");
		result.Single().ClosedActionsAndDecisions[1].Should().Be("2");
		result.Single().ClosedActionsAndDecisions[2].Should().Be("3");
	}
	
	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_ReturnsCasesSortedByCreatedDateDescending()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildListClosedCaseSummaryDtos();

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName))
			.ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(data.Count);
		var sortedData = data.OrderByDescending(d => d.CreatedAt);
		result.Select(r => r.CreatedAt).Should().ContainInConsecutiveOrder(sortedData.Select(r => r.CreatedAt.ToDayMonthYear()));
	}

	[Test]
	public async Task GetClosedCaseSummariesByCaseworker_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirst()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName);
		data.ClosedConcerns = new List<CaseSummaryDto.ConcernSummaryDto>
		{
			new("2", new RatingDto("Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-5)),
			new("5", new RatingDto("Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-2)),
			new("3", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-4)),
			new("6", new RatingDto("Red-Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-1)),
			new("1", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-6)),
			new("4", new RatingDto("Amber-Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-3))
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByCaseworker(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByCaseworker(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ClosedConcerns.Select(int.Parse).Should().BeInAscendingOrder();
	}

	#endregion
	
	#region Closed by Trust
	[Test]
	public async Task GetClosedCaseSummariesByTrust_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		mockCaseSummaryService.Setup(s => s.GetClosedCaseSummariesByTrust(userName)).ReturnsAsync(new List<ClosedCaseSummaryDto>());
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Should().BeEmpty();
	}
	
	
	[Test]
	public async Task GetClosedCaseSummariesByTrust_WhenCases_ReturnsList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = BuildListClosedCaseSummaryDtos();
		mockCaseSummaryService.Setup(s => s.GetClosedCaseSummariesByTrust(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
		
	[Test]
	public async Task GetClosedCaseSummariesByTrust_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"), 
			new(DateTime.Now, null, "some name 2"), 
			new(DateTime.Now, null, "some name 3"), 
			new(DateTime.Now, null, "some name 4")
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeTrue();
	}
	
	[Test]
	public async Task GetClosedCaseSummariesByTrust_WhenCasesWith3OrFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeFalse();
	}

	[Test]
	public async Task GetClosedCaseSummariesByTrust_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsNoMoreThan3ActionsAndDecisionsInCreatedDateOrder()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new CaseSummaryDto.ActionDecisionSummaryDto[]
		{
			new(DateTime.Now.AddDays(-3), null, "1"),
			new(DateTime.Now, null, "Not returned"),
			new(DateTime.Now.AddDays(-2), null, "2"), 
			new(DateTime.Now.AddDays(-1), null, "3")
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Single().ClosedActionsAndDecisions.Length.Should().Be(3);
		result.Single().ClosedActionsAndDecisions[0].Should().Be("1");
		result.Single().ClosedActionsAndDecisions[1].Should().Be("2");
		result.Single().ClosedActionsAndDecisions[2].Should().Be("3");
	}
	
	[Test]
	public async Task GetClosedCaseSummariesByTrust_ReturnsCasesSortedByCreatedDateDescending()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildListClosedCaseSummaryDtos();

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByTrust(userName))
			.ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Count.Should().Be(data.Count);
		var sortedData = data.OrderByDescending(d => d.CreatedAt);
		result.Select(r => r.CreatedAt).Should().ContainInConsecutiveOrder(sortedData.Select(r => r.CreatedAt.ToDayMonthYear()));
	}

	[Test]
	public async Task GetClosedCaseSummariesByTrust_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirst()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildClosedCaseSummaryDto(userName);
		data.ClosedConcerns = new List<CaseSummaryDto.ConcernSummaryDto>
		{
			new("2", new RatingDto("Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-5)),
			new("5", new RatingDto("Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-2)),
			new("3", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-4)),
			new("6", new RatingDto("Red-Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-1)),
			new("1", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-6)),
			new("4", new RatingDto("Amber-Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now.AddDays(-3))
		};

		mockCaseSummaryService
			.Setup(s => s.GetClosedCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ClosedCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetClosedCaseSummariesByTrust(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ClosedConcerns.Select(int.Parse).Should().BeInAscendingOrder();
	}

	#endregion
	
	#region Active by Trust
	[Test]
	public async Task GetActiveCaseSummariesByTrust_WhenNoCases_ReturnsEmptyList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByTrust(userName)).ReturnsAsync(new List<ActiveCaseSummaryDto>());
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Should().BeEmpty();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByTrust_WhenCases_ReturnsList()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();

		var data = BuildListActiveCaseSummaryDtos();
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByTrust(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
		
	[Test]
	public async Task GetActiveCaseSummariesByTrust_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsTrue()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"), 
			new(DateTime.Now, null, "some name 2"), 
			new(DateTime.Now, null, "some name 3"), 
			new(DateTime.Now, null, "some name 4")
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeTrue();
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByTrust_WhenCasesWith3OrFewerDecisionsAndActions_ReturnsListWithIsMoreActionsAndDecisionsFalse()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>
		{
			new(DateTime.Now, null, "some name 1"),
			new(DateTime.Now, null, "some name 2"),
			new(DateTime.Now, null, "some name 3")
		};
		
		data.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		data.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Single().IsMoreActionsAndDecisions.Should().BeFalse();
	}

	[Test]
	public async Task GetActiveCaseSummariesByTrust_WhenCasesWithMoreThan3DecisionsAndActions_ReturnsNoMoreThan3ActionsAndDecisionsInCreatedDateOrder()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName, emptyActionsAndDecisions: true);
		data.Decisions = new CaseSummaryDto.ActionDecisionSummaryDto[]
		{
			new(DateTime.Now.AddDays(-3), null, "1"),
			new(DateTime.Now, null, "Not returned"),
			new(DateTime.Now.AddDays(-2), null, "2"), 
			new(DateTime.Now.AddDays(-1), null, "3")
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{data});
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Single().ActiveActionsAndDecisions.Length.Should().Be(3);
		result.Single().ActiveActionsAndDecisions[0].Should().Be("1");
		result.Single().ActiveActionsAndDecisions[1].Should().Be("2");
		result.Single().ActiveActionsAndDecisions[2].Should().Be("3");
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByTrust_ReturnsCasesSortedByCreatedDateDescending()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildListActiveCaseSummaryDtos();

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Count.Should().Be(data.Count);
		var sortedData = data.OrderByDescending(d => d.CreatedAt);
		result.Select(r => r.CreatedAt).Should().ContainInConsecutiveOrder(sortedData.Select(r => r.CreatedAt.ToDayMonthYear()));
	}
	
	[Test]
	public async Task GetActiveCaseSummariesByTrust_ReturnsCasesWithConcernsSortedByRagRating()
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName);
		data.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto> { 
			new("4", new RatingDto("Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("2", new RatingDto("Red", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("1", new RatingDto("Red-Plus", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("5", new RatingDto("Amber-Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("6", new RatingDto("Green", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now),
			new("3", new RatingDto("Red-Amber", DateTimeOffset.Now, DateTimeOffset.Now, 1), DateTime.Now)
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInAscendingOrder();
	}

	[Test]
	[TestCase("Amber")]
	[TestCase("Red")]
	[TestCase("Red-Amber")]
	[TestCase("Amber-Green")]
	[TestCase("Red-Plus")]
	[TestCase("Green")]
	public async Task GetActiveCaseSummariesByTrust_ReturnsCasesWithConcernsSortedByCreatedAtOldestFirstWhenRagRatingsTheSame(string ragRating)
	{
		// arrange
		var mockCaseSummaryService = new Mock<IApiCaseSummaryService>();
		var trustCachedService = new Mock<ITrustCachedService>();

		var userName = _fixture.Create<string>();
		var data = BuildActiveCaseSummaryDto(userName);
		var ratingDto = new RatingDto(ragRating, DateTimeOffset.Now, DateTimeOffset.Now, 1);
		data.ActiveConcerns = new List<CaseSummaryDto.ConcernSummaryDto>
		{
			new("4", ratingDto, DateTime.Now.AddDays(-4)),
			new("3", ratingDto, DateTime.Now.AddDays(-3)),
			new("2", ratingDto, DateTime.Now.AddDays(-2)),
			new("5", ratingDto, DateTime.Now.AddDays(-5)),
			new("1", ratingDto, DateTime.Now.AddDays(-1)), // newest should be last
			new("6", ratingDto, DateTime.Now.AddDays(-6)) // oldest should be first
		};

		mockCaseSummaryService
			.Setup(s => s.GetActiveCaseSummariesByTrust(userName))
			.ReturnsAsync(new List<ActiveCaseSummaryDto>{ data });
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByTrust(userName);

		// assert
		result.Count.Should().Be(1);
		result.Single().ActiveConcerns.Select(int.Parse).Should().BeInDescendingOrder();
	}

	#endregion
	
	private List<ActiveCaseSummaryDto> BuildListActiveCaseSummaryDtos()
		=> _fixture.CreateMany<ActiveCaseSummaryDto>().ToList();
	
	private List<ActiveCaseSummaryDto> BuildListActiveCaseSummaryDtos(string userName)
		=> _fixture.Build<ActiveCaseSummaryDto>()
			.With(d => d.CreatedBy, userName)
			.CreateMany().ToList();

	private ActiveCaseSummaryDto BuildActiveCaseSummaryDto(string userName, bool emptyActionsAndDecisions = false)
	{
		var dto = _fixture.Build<ActiveCaseSummaryDto>()
			.With(d => d.CreatedBy, userName)
			.Create();

		if (!emptyActionsAndDecisions) return dto;
		
		dto.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.TrustFinancialForecasts = new List<CaseSummaryDto.ActionDecisionSummaryDto>();

		return dto;
	}
	
	private List<ClosedCaseSummaryDto> BuildListClosedCaseSummaryDtos()
		=> _fixture.CreateMany<ClosedCaseSummaryDto>().ToList();

	private ClosedCaseSummaryDto BuildClosedCaseSummaryDto(string userName, bool emptyActionsAndDecisions = false)
	{
		var dto = _fixture
			.Build<ClosedCaseSummaryDto>()
			.With(d => d.CreatedBy, userName)
			.Create();

		if (!emptyActionsAndDecisions) return dto;
		
		dto.Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		dto.TrustFinancialForecasts = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		
		return dto;
	}
}