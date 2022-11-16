using AutoFixture;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Services.Cases;
using FluentAssertions;
using Moq;
using NUnit.Framework;
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

		var data = _fixture.CreateMany<ActiveCaseSummaryDto>().ToList();
		mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesByCaseworker(userName)).ReturnsAsync(data);
		
		var sut = new CaseSummaryService(mockCaseSummaryService.Object, trustCachedService.Object);

		// act
		var result = await sut.GetActiveCaseSummariesByCaseworker(userName);

		// assert
		result.Should().HaveCount(data.Count);
	}
}