using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.TrustFinancialForecasts;

public class UpdateTrustFinancialForecastTests
{
	private readonly IFixture _fixture;

	public UpdateTrustFinancialForecastTests()
	{
		_fixture = new Fixture();
		_fixture.Register(Mock.Of<IConcernsCaseGateway>);
	}
	
	[Fact]
	public void UpdateTrustFinancialForecasts_Is_Assignable_To_IUseCaseAsync()
	{
		var sut = new UpdateTrustFinancialForecast(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());

		sut.Should()
			.BeAssignableTo<IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>>();
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Not_Found_Throws_Exception()
	{
		// arrange 
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var trustFinancialForecastId = _fixture.Create<int>();
		
		var request = _fixture.Build<UpdateTrustFinancialForecastRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.With(x => x.TrustFinancialForecastId, trustFinancialForecastId)
			.Create();

		mockCaseGateWay.Setup(x => x.GetConcernsCaseByUrn(caseUrn, It.IsAny<bool>())).Returns(default(ConcernsCase));

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<NotFoundException>())
			.And.Message.Should().Be($"Concerns Case {caseUrn} not found");
	}
	
	[Fact]
	public async Task Execute_When_TrustFinancialForecast_Not_Found_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var trustFinancialForecastId = _fixture.Create<int>();
		
		var request = _fixture.Build<UpdateTrustFinancialForecastRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.With(x => x.TrustFinancialForecastId, trustFinancialForecastId)
			.Create();

		mockCaseGateWay
			.Setup(x => x.CaseExists(caseUrn, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<NotFoundException>())
			.And.Message.Should().Be($"Trust Financial Forecast with Id {trustFinancialForecastId} not found");
	}
	
	[Fact]
	public async Task Execute_When_TrustFinancialForecastId_Empty_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var notes = _fixture.Create<string>();
		
		var request = new UpdateTrustFinancialForecastRequest { CaseUrn = caseUrn, Notes = notes };

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>())
			.And.Message.Should().Be("Request is not valid (Parameter 'request')");
	}
	
	[Fact]
	public async Task Execute_When_TrustFinancialForecastId_Invalid_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var notes = _fixture.Create<string>();
		
		var request = new UpdateTrustFinancialForecastRequest { CaseUrn = caseUrn, TrustFinancialForecastId = 0, Notes = notes };

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>())
			.And.Message.Should().Be("Request is not valid (Parameter 'request')");
	}
			
	[Fact]
	public async Task Execute_When_CaseUrn_Empty_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var notes = _fixture.Create<string>();
		
		var request = new UpdateTrustFinancialForecastRequest { TrustFinancialForecastId = trustFinancialForecastId, Notes = notes };

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>())
			.And.Message.Should().Be("Request is not valid (Parameter 'request')");
	}
				
	[Fact]
	public async Task Execute_When_CaseUrn_Invalid_Throws_Exception()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var notes = _fixture.Create<string>();
		
		var request = new UpdateTrustFinancialForecastRequest { CaseUrn = 0, TrustFinancialForecastId = trustFinancialForecastId, Notes = notes };

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentException>())
			.And.Message.Should().Be("Request is not valid (Parameter 'request')");
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_And_TrustFinancialForecast_Found_Updates_TrustFinancialForecast()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var id = _fixture.Create<int>();
		var trustFinancialForecast = CreateOpenTrustFinancialForecast(caseUrn, id);
		
		var request = CreateUpdateTrustFinancialForecastRequest(caseUrn, id);
		
		mockCaseGateWay.Setup(x => x.CaseExists(caseUrn, It.IsAny<CancellationToken>())).ReturnsAsync(true);
		mockTrustFinancialForecastGateway
			.Setup(x => x.GetById(
				It.Is<int>(r => r == id), 
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(trustFinancialForecast);
		mockTrustFinancialForecastGateway.Setup(x => x.Update(It.IsAny<TrustFinancialForecast>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(id);

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var result = await sut.Execute(request, CancellationToken.None);

		// assert
		result.Should().Be(id);
	}

	[Fact]
	public async Task Execute_When_TrustFinancialForecast_Closed_Should_ThrowException()
	{
		// arrange
		var mockTrustFinancialForecastGateway = new Mock<ITrustFinancialForecastGateway>();
		var mockCaseGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		var id = _fixture.Create<int>();
		var trustFinancialForecast = CreateClosedTrustFinancialForecast(caseUrn, id);
		
		var request = CreateUpdateTrustFinancialForecastRequest(caseUrn, id);
		
		mockCaseGateWay.Setup(x => x.CaseExists(caseUrn, It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		mockTrustFinancialForecastGateway
			.Setup(x => x.GetById(
				It.Is<int>(r => r == id), 
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(trustFinancialForecast);
		
		mockTrustFinancialForecastGateway.Setup(x => x.Update(It.IsAny<TrustFinancialForecast>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(id);

		var sut = new UpdateTrustFinancialForecast(mockCaseGateWay.Object, mockTrustFinancialForecastGateway.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<StateChangeNotAllowedException>())
			.And.Message.Should().Be($"Cannot update Trust Financial Forecast with Id {id} as it is closed.");
	}

	[Fact]
	public void Constructor_Guards_Against_Null_Arguments()
	{
		_fixture.Customize(new AutoMoqCustomization());
		
		var assertion = _fixture.Create<GuardClauseAssertion>();
		
		assertion.Verify(typeof(UpdateTrustFinancialForecast).GetConstructors());
	}

	[Fact]
	public async Task Methods_Guards_Against_Null_Arguments()
	{
		var sut = new UpdateTrustFinancialForecast(Mock.Of<IConcernsCaseGateway>(), Mock.Of<ITrustFinancialForecastGateway>());
		
		var action = () => sut.Execute(null, CancellationToken.None);

		(await action.Should().ThrowAsync<ArgumentNullException>())
			.And.Message.Should().Be("Value cannot be null. (Parameter 'request')");
	}

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
	
	private UpdateTrustFinancialForecastRequest CreateUpdateTrustFinancialForecastRequest(int caseUrn, int trustFinancialForecastId) 
		=> _fixture.Build<UpdateTrustFinancialForecastRequest>()
			.With(x => x.CaseUrn, caseUrn)
			.With(x => x.TrustFinancialForecastId, trustFinancialForecastId)
			.Create();
}