using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers;

public class TrustFinancialForecastControllerTests
{
	[Fact]
	public void Constructor_Guards_Against_Null_Arguments()
	{
		// Arrange
		var fixture = new Fixture();
		fixture.Customize(new AutoMoqCustomization());
		fixture.Register(() => Mock.Of<ILogger<TrustFinancialForecastController>>());

		var assertion = fixture.Create<GuardClauseAssertion>();

		// Act & Assert
		assertion.Verify(typeof(TrustFinancialForecastController).GetConstructors());
	}

	[Fact]
	public async Task CreateConcernsCaseTrustFinancialForecast_Returns201WhenSuccessfullyCreatesAConcernsCase()
	{
		// arrange
		var testBuilder = new TestBuilder();

		var request = testBuilder._fixture.Create<CreateTrustFinancialForecastRequest>();
		var response = testBuilder._fixture.Create<int>();

		testBuilder._createTrustFinancialForecastUseCase.Setup(a => a.Execute(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.Create(123, request, CancellationToken.None);

		// assert
		var expected = new ApiSingleResponseV2<string>(response.ToString());
		result.Result.Should().BeEquivalentTo(new ObjectResult(expected) { StatusCode = StatusCodes.Status201Created });
	}

	[Fact]
	public async Task CreateConcernsCaseTrustFinancialForecast_ReturnsBadRequest_When_CreateTrustFinancialForecastRequest_IsInvalid()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateInvalidCreateTrustFinancialForecastRequest();
		var response = testBuilder._fixture.Create<int>();

		testBuilder._createTrustFinancialForecastUseCase.Setup(a => a.Execute(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);

		var sut = testBuilder.BuildSut();		
		
		// act
		var result = await sut.Create(123, request, CancellationToken.None);

		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task CreateConcernsCaseTrustFinancialForecast_ReturnsBadRequest_When_CreateTrustFinancialForecastRequest_IsNull()
	{
		// arrange
		var testBuilder = new TestBuilder();

		var sut = testBuilder.BuildSut();		
		
		// act
		var result = await sut.Create(123, null, CancellationToken.None);

		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task CreateConcernsCaseTrustFinancialForecast_When_Invalid_Urn_Returns_BadRequest(int urn)
	{
		// arrange
		var testBuilder = new TestBuilder();

		var request = testBuilder._fixture.Create<CreateTrustFinancialForecastRequest>();

		var response = testBuilder._fixture.Create<int>();

		testBuilder._createTrustFinancialForecastUseCase.Setup(a => a.Execute(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);

		var sut = testBuilder.BuildSut();		
		
		// act
		var result = await sut.Create(urn, request, CancellationToken.None);

		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task GetById_When_Invalid_Urn_Returns_BadRequest(int urn)
	{
		// arrange
		var testBuilder = new TestBuilder();

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.GetById(urn, 123, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task GetById_When_Invalid_TrustFinancialForecastId_Returns_BadRequest(int trustFinancialForecastId)
	{
		// arrange
		var testBuilder = new TestBuilder();

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.GetById(123, trustFinancialForecastId, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task GetById_When_Valid_TrustFinancialForecastId_Returns_TrustFinancialForecastResponse()
	{
		// arrange
		const int expectedCaseUrn = 123;
		const int expectedTrustFinancialForecastId = 456;

		var testBuilder = new TestBuilder();

		var expectedTrustFinancialForecastResponse = testBuilder._fixture.Build<TrustFinancialForecastResponse>()
			.With(x => x.TrustFinancialForecastId, expectedTrustFinancialForecastId)
			.Create();

		testBuilder._getTrustFinancialForecastUseCase.Setup(x => x.Execute(It.Is<GetTrustFinancialForecastByIdRequest>(r => r.CaseUrn == expectedCaseUrn && r.TrustFinancialForecastId == expectedTrustFinancialForecastId), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTrustFinancialForecastResponse);

		// Act
		var sut = testBuilder.BuildSut();
		var actionResult = await sut.GetById(expectedCaseUrn, expectedTrustFinancialForecastId, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
		var expectedOkResult = new OkObjectResult(new ApiSingleResponseV2<TrustFinancialForecastResponse>(expectedTrustFinancialForecastResponse));
		okResult.Should().BeEquivalentTo(expectedOkResult);
	}

	[Fact]
	public async Task GetById_When_TrustFinancialForecastNotFound_Returns_NotFound()
	{
		// arrange
		const int expectedCaseUrn = 123;
		const int expectedTrustFinancialForecastId = 456;

		var testBuilder = new TestBuilder();

		testBuilder._getTrustFinancialForecastUseCase.Setup(x => x.Execute(It.Is<GetTrustFinancialForecastByIdRequest>(r => r.CaseUrn == expectedCaseUrn && r.TrustFinancialForecastId == expectedTrustFinancialForecastId), It.IsAny<CancellationToken>()))
			.ReturnsAsync(default(TrustFinancialForecastResponse));

		// Act
		var sut = testBuilder.BuildSut();
		var actionResult = await sut.GetById(expectedCaseUrn, expectedTrustFinancialForecastId, CancellationToken.None);

		// Assert
		Assert.IsType<NotFoundResult>(actionResult.Result);
	}

	[Fact]
	public async Task GetTrustFinancialForecasts_When_Invalid_Urn_Returns_BadRequest()
	{
		// arrange
		var testBuilder = new TestBuilder();

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.GetAllForCase(0, CancellationToken.None);
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task GetTrustFinancialForecasts_When_Null_Response_Returns_NotFound()
	{
		// arrange
		var testBuilder = new TestBuilder();
		testBuilder._getTrustFinancialForecastsUseCase
			.Setup(x => x.Execute(It.IsAny<GetTrustFinancialForecastsForCaseRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(default(TrustFinancialForecastResponse[]));

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.GetAllForCase(testBuilder._fixture.Create<int>(), CancellationToken.None);
		
		// assert
		Assert.IsType<NotFoundResult>(result.Result);
	}

	[Fact]
	public async Task GetTrustFinancialForecasts_When_TrustFinancialForecasts_Found_Returns_ApiSingleResponseV2_Of_TrustFinancialForecastSummaryArray()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var expectedDtos = testBuilder._fixture.CreateMany<TrustFinancialForecastResponse>(2).ToArray();

		testBuilder._getTrustFinancialForecastsUseCase
			.Setup(x => x.Execute(It.IsAny<GetTrustFinancialForecastsForCaseRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedDtos);

		var sut = testBuilder.BuildSut();

		// act
		var response = await sut.GetAllForCase(testBuilder._fixture.Create<int>(), CancellationToken.None);
		
		// assert
		var okResult = Assert.IsType<OkObjectResult>(response.Result);
		var expectedOkResult = new OkObjectResult(new ApiSingleResponseV2<TrustFinancialForecastResponse[]>(expectedDtos));
		okResult.Should().BeEquivalentTo(expectedOkResult);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task UpdateTrustFinancialForecast_When_Invalid_Urn_Returns_BadRequest(int urn)
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateValidUpdateTrustFinancialForecastRequest();

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.Update(urn, 123, request, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task UpdateTrustFinancialForecast_When_Invalid_TrustFinancialForecastId_Returns_BadRequest(int trustFinancialForecastId)
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateValidUpdateTrustFinancialForecastRequest();

		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.Update(123, trustFinancialForecastId, request, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task UpdateTrustFinancialForecast_When_Invalid_Request_Returns_BadRequest()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateInvalidUpdateTrustFinancialForecastRequest();
		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.Update(123, 456, request, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task UpdateTrustFinancialForecast_When_Valid_Arguments_Returns_OkResponse()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateValidUpdateTrustFinancialForecastRequest();

		var expectedResponse = testBuilder._fixture.Create<int>();
		testBuilder._updateTrustFinancialForecastUseCase.Setup(x =>
				x.Execute(It.IsAny<UpdateTrustFinancialForecastRequest>(),
					It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResponse);

		var sut = testBuilder.BuildSut();
		
		// act
		var actionResult = await sut.Update(123, 456, request, CancellationToken.None);
		
		// assert
		var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
		var expectedOkResult = new OkObjectResult(new ApiSingleResponseV2<string>(expectedResponse.ToString()));
		okResult.Should().BeEquivalentTo(expectedOkResult);
	}
        
	[Fact]
	public async Task CloseTrustFinancialForecast_When_Invalid_Request_Returns_BadRequest()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateCloseTrustFinancialForecastRequestNotesTooLong();
		var sut = testBuilder.BuildSut();
		
		// act
		var result = await sut.Close(1, 2, request, CancellationToken.None);
		
		// assert
		result.Result.Should().BeEquivalentTo(new BadRequestResult());
	}

	[Fact]
	public async Task CloseTrustFinancialForecast_When_Valid_Arguments_Returns_OkResponse()
	{
		// arrange
		var testBuilder = new TestBuilder();
		var request = testBuilder.CreateValidCloseTrustFinancialForecastRequest();

		var expectedResponse = testBuilder._fixture.Create<int>();
		testBuilder._closeTrustFinancialForecastUseCase
			.Setup(x => x.Execute(request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResponse);

		var sut = testBuilder.BuildSut();
		
		// act
		var actionResult = await sut.Close(request.CaseUrn, request.TrustFinancialForecastId, request, CancellationToken.None);
		
		// assert
		var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
		okResult.Value.Should().BeAssignableTo<ApiSingleResponseV2<string>>();
		var response = (ApiSingleResponseV2<string>)okResult.Value;
		response!.Data.Should().Be(expectedResponse.ToString());
	}
        
	private class TestBuilder
	{
		internal readonly Fixture _fixture;
		internal readonly Mock<ILogger<TrustFinancialForecastController>> _mockLogger;
		internal readonly Mock<IUseCaseAsync<CreateTrustFinancialForecastRequest, int>> _createTrustFinancialForecastUseCase;
		internal readonly Mock<IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse>> _getTrustFinancialForecastUseCase;
		internal readonly Mock<IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>> _getTrustFinancialForecastsUseCase;
		internal readonly Mock<IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>> _updateTrustFinancialForecastUseCase;
		internal readonly Mock<IUseCaseAsync<CloseTrustFinancialForecastRequest, int>> _closeTrustFinancialForecastUseCase;

		public TestBuilder()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());

			_mockLogger = new Mock<ILogger<TrustFinancialForecastController>>();
			_createTrustFinancialForecastUseCase = _fixture.Freeze<Mock<IUseCaseAsync<CreateTrustFinancialForecastRequest, int>>>();
			_getTrustFinancialForecastUseCase = _fixture.Freeze<Mock<IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse>>>();
			_getTrustFinancialForecastsUseCase = _fixture.Freeze<Mock<IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>>>();
			_updateTrustFinancialForecastUseCase = _fixture.Freeze<Mock<IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>>>();
			_closeTrustFinancialForecastUseCase = _fixture.Freeze<Mock<IUseCaseAsync<CloseTrustFinancialForecastRequest, int>>>();
		}

		internal TrustFinancialForecastController BuildSut()
		{
			return new TrustFinancialForecastController(_mockLogger.Object, 
				_createTrustFinancialForecastUseCase.Object, 
				_getTrustFinancialForecastUseCase.Object, 
				_updateTrustFinancialForecastUseCase.Object,
				_closeTrustFinancialForecastUseCase.Object,
				_getTrustFinancialForecastsUseCase.Object );
		}

		public UpdateTrustFinancialForecastRequest CreateValidUpdateTrustFinancialForecastRequest()
		{
			return _fixture.Create<UpdateTrustFinancialForecastRequest>();
		}
		
		public UpdateTrustFinancialForecastRequest CreateInvalidUpdateTrustFinancialForecastRequest()
		{
			return _fixture.Build<UpdateTrustFinancialForecastRequest>()
				.With(x => x.Notes,  "test".PadLeft(2001, 'a'))
				.Create();
		}
            
		public CloseTrustFinancialForecastRequest CreateValidCloseTrustFinancialForecastRequest()
		{
			return _fixture.Create<CloseTrustFinancialForecastRequest>();
		}
		
		public CloseTrustFinancialForecastRequest CreateCloseTrustFinancialForecastRequestNotesTooLong()
		{
			return _fixture.Build<CloseTrustFinancialForecastRequest>()
				.With(x => x.Notes,  "test".PadLeft(2001, 'a'))
				.Create();
		}
		
		public CreateTrustFinancialForecastRequest CreateInvalidCreateTrustFinancialForecastRequest()
		{
			return _fixture.Build<CreateTrustFinancialForecastRequest>()
				.With(x => x.Notes, "a".PadLeft(2001, '-'))
				.Create();
		}
	}
}