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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.TrustFinancialForecasts;

public class CreateTrustFinancialForecastTests
{
	private readonly IFixture _fixture;

	public CreateTrustFinancialForecastTests()
	{
		_fixture = new Fixture();
		_fixture.Register(Mock.Of<IConcernsCaseGateway>);
	}
	
	[Fact]
	public void CreateTrustFinancialForecasts_Is_Assignable_To_IUseCaseAsync()
	{
		var sut = new CreateTrustFinancialForecast(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());

		sut.Should()
			.BeAssignableTo<IUseCaseAsync<CreateTrustFinancialForecastRequest, int>>();
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Not_Found_Throws_Exception()
	{
		// arrange 
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		
		var request = _fixture.Build<CreateTrustFinancialForecastRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.Create();

		mockCaseGateWay.Setup(x => x.GetConcernsCaseByUrn(caseUrn, It.IsAny<bool>())).Returns(default(ConcernsCase));

		var sut = new CreateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
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
		
		var request = CreateCreateTrustFinancialForecastRequest(null);

		var sut = new CreateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
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

		var notes = _fixture.Create<string>();
		
		var request = new CreateTrustFinancialForecastRequest { CaseUrn = 0, Notes = notes };

		var sut = new CreateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>()).And.Message.Should()
			.Be("Request is not valid (Parameter 'request')");
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Found_Creates_TrustFinancialForecast()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();
		
		var caseUrn = _fixture.Create<int>();
		var trustFinancialForecastId = _fixture.Create<int>();
		
		var request = CreateCreateTrustFinancialForecastRequest(caseUrn);
		
		mockCaseGateWay.Setup(x => x.CaseExists(caseUrn, It.IsAny<CancellationToken>())).ReturnsAsync(true);
		mockTrustFinancialForecastGateway
			.Setup(x => x.Update(It.IsAny<TrustFinancialForecast>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(trustFinancialForecastId);

		var sut = new CreateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var result = await sut.Execute(request, CancellationToken.None);

		// assert
		result.Should().Be(trustFinancialForecastId);
	}

	[Fact]
	public void Constructor_Guards_Against_Null_Arguments()
	{
		_fixture.Customize(new AutoMoqCustomization());
		
		var assertion = _fixture.Create<GuardClauseAssertion>();
		
		assertion.Verify(typeof(CreateTrustFinancialForecast).GetConstructors());
	}

	[Fact]
	public async Task Methods_Guards_Against_Null_Arguments()
	{
		var sut = new CreateTrustFinancialForecast(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());
		
		var action = () => sut.Execute(null, CancellationToken.None);

		(await action.Should().ThrowAsync<ArgumentNullException>()).And.Message.Should()
			.Be("Value cannot be null. (Parameter 'request')");
	}

	private CreateTrustFinancialForecastRequest CreateCreateTrustFinancialForecastRequest(int? caseUrn) 
		=> _fixture.Build<CreateTrustFinancialForecastRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.Create();
	
	private TrustFinancialForecast CreateOpenTrustFinancialForecast(int caseUrn, int id) 
		=> _fixture
			.Build<TrustFinancialForecast>()
			.Without(x => x.ClosedAt)
			.With(x => x.CaseUrn, caseUrn)
			.With(x => x.Id, id)
			.Create();
	
	private TrustFinancialForecast CreateClosedTrustFinancialForecast(int caseUrn, int id) 
		=> _fixture.Build<TrustFinancialForecast>()
			.With(x => x.CaseUrn, caseUrn)
			.With(x => x.Id, id)
			.Create();
}