using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.TrustFinancialForecasts;

public class GetTrustFinancialForecastsForCaseTests
{
	private readonly IFixture _fixture;

	public GetTrustFinancialForecastsForCaseTests()
	{
		_fixture = new Fixture();
		_fixture.Register(Mock.Of<IConcernsCaseGateway>);
	}
	
	[Fact]
	public void GetTrustFinancialForecastsForCases_Is_Assignable_To_IUseCaseAsync()
	{
		var sut = new GetTrustFinancialForecastsForCase(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());

		sut.Should()
			.BeAssignableTo<IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>>();
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Not_Found_Throws_Exception()
	{
		// arrange 
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		
		var request = _fixture.Build<GetTrustFinancialForecastsForCaseRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.Create();

		mockCaseGateWay.Setup(x => x.GetConcernsCaseByUrn(caseUrn, It.IsAny<bool>())).Returns(default(ConcernsCase));

		var sut = new GetTrustFinancialForecastsForCase(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<NotFoundException>()).And.Message.Should()
			.Be($"Concerns Case {caseUrn} not found");
	}

	[Fact]
	public async Task Execute_When_CaseUrn_Empty_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();
		
		var request = new GetTrustFinancialForecastsForCaseRequest();

		var sut = new GetTrustFinancialForecastsForCase(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>()).And.Message.Should()
			.Be("Request is not valid (Parameter 'request')");
	}
				
	[Fact]
	public async Task Execute_When_CaseUrn_Invalid_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();
		
		var request = new GetTrustFinancialForecastsForCaseRequest { CaseUrn = 0 };

		var sut = new GetTrustFinancialForecastsForCase(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>()).And.Message.Should()
			.Be("Request is not valid (Parameter 'request')");
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Found_Gets_TrustFinancialForecast()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var trustFinancialForecast = new List<TrustFinancialForecastResponse> { CreateOpenTrustFinancialForecast() };
		var caseUrn = trustFinancialForecast.First().CaseUrn;
		
		var request = new GetTrustFinancialForecastsForCaseRequest { CaseUrn = caseUrn };
		mockTrustFinancialForecastGateway
			.Setup(x => x.GetAllForCase(
				It.Is<GetTrustFinancialForecastsForCaseRequest>(r => r.CaseUrn == caseUrn), 
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(trustFinancialForecast);
		
		mockCaseGateWay.Setup(x => x.CaseExists(caseUrn, It.IsAny<CancellationToken>())).ReturnsAsync(true);

		var sut = new GetTrustFinancialForecastsForCase(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var result = await sut.Execute(request, CancellationToken.None);

		// assert
		result.Should().BeEquivalentTo(trustFinancialForecast);
	}

	[Fact]
	public void Constructor_Guards_Against_Null_Arguments()
	{
		_fixture.Customize(new AutoMoqCustomization());
		
		var assertion = _fixture.Create<GuardClauseAssertion>();
		
		assertion.Verify(typeof(GetTrustFinancialForecastsForCase).GetConstructors());
	}

	[Fact]
	public async Task Methods_Guards_Against_Null_Arguments()
	{
		var sut = new GetTrustFinancialForecastsForCase(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());
		
		var action = () => sut.Execute(null, CancellationToken.None);

		(await action.Should().ThrowAsync<ArgumentNullException>()).And.Message.Should()
			.Be("Value cannot be null. (Parameter 'request')");
	}

	private TrustFinancialForecastResponse CreateOpenTrustFinancialForecast() => _fixture.Build<TrustFinancialForecastResponse>().Without(x => x.ClosedAt).Create();
}