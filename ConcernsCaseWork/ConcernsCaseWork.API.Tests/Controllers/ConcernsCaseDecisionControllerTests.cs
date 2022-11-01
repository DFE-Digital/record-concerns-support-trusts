using System.Linq;
using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Enums.Concerns;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
    public class ConcernsCaseDecisionControllerTests
    {
        private readonly Mock<ILogger<ConcernsCaseDecisionController>> _mockLogger = new Mock<ILogger<ConcernsCaseDecisionController>>();

        [Fact]
        public void Constructor_Guards_Against_Null_Arguments()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register(() => Mock.Of<ILogger<ConcernsCaseDecisionController>>());
            fixture.Register(() => Mock.Of<IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse>>());
            fixture.Register(() => Mock.Of<IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>>());
            fixture.Register(() => Mock.Of<IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>>());

            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(ConcernsCaseDecisionController).GetConstructors());
        }

        [Fact]
        public async Task CreateConcernsCaseDecision_Returns201WhenSuccessfullyCreatesAConcernsCase()
        {
            var testBuilder = new TestBuilder();

            var createDecisionRequest = testBuilder.Fixture.Create<CreateDecisionRequest>();
            var createDecisionResponse = testBuilder.Fixture.Create<CreateDecisionResponse>();

            testBuilder.CreateDecisionUseCase.Setup(a => a.Execute(createDecisionRequest, It.IsAny<CancellationToken>())).ReturnsAsync(createDecisionResponse);

            var sut = testBuilder.BuildSut();
            var result = await sut.Create(123, createDecisionRequest, CancellationToken.None);

            var expected = new ApiSingleResponseV2<CreateDecisionResponse>(createDecisionResponse);
            result.Result.Should().BeEquivalentTo(new ObjectResult(expected) { StatusCode = StatusCodes.Status201Created });
        }


        [Fact]
        public async Task CreateConcernsCaseDecision_ReturnsBadRequest_When_CreateDecisionRequest_IsInvalid()
        {
            var testBuilder = new TestBuilder();

            var createDecisionRequest = testBuilder.Fixture.Build<CreateDecisionRequest>()
                .With(x => x.DecisionTypes, () => new DecisionType[] { 0 })
                .Create();

            var createDecisionResponse = testBuilder.Fixture.Create<CreateDecisionResponse>();

            testBuilder.CreateDecisionUseCase.Setup(a => a.Execute(createDecisionRequest, It.IsAny<CancellationToken>())).ReturnsAsync(createDecisionResponse);

            var sut = testBuilder.BuildSut();
            var result = await sut.Create(123, createDecisionRequest, CancellationToken.None);

            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task CreateConcernsCaseDecision_ReturnsBadRequest_When_CreateDecisionRequest_IsNull()
        {
            var testBuilder = new TestBuilder();

            var sut = testBuilder.BuildSut();
            var result = await sut.Create(123, null, CancellationToken.None);

            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task CreateConcernsCaseDecision_ReturnsBadRequest_When_Urn_Is_Zero()
        {
            var testBuilder = new TestBuilder();

            var createDecisionRequest = testBuilder.Fixture.Build<CreateDecisionRequest>()
                .With(x => x.DecisionTypes, () => new DecisionType[] { DecisionType.EsfaApproval })
                .Create();

            var createDecisionResponse = testBuilder.Fixture.Create<CreateDecisionResponse>();

            testBuilder.CreateDecisionUseCase.Setup(a => a.Execute(createDecisionRequest, It.IsAny<CancellationToken>())).ReturnsAsync(createDecisionResponse);

            var sut = testBuilder.BuildSut();
            var result = await sut.Create(0, createDecisionRequest, CancellationToken.None);

            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task GetById_When_Invalid_Urn_Returns_BadRequest()
        {
            var testBuilder = new TestBuilder();

            var sut = testBuilder.BuildSut();

            var result = await sut.GetById(0, 123, CancellationToken.None);
            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task GetById_When_Invalid_DecisionId_Returns_BadRequest()
        {
            var testBuilder = new TestBuilder();

            var sut = testBuilder.BuildSut();

            var result = await sut.GetById(123, 0, CancellationToken.None);
            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task GetById_When_Valid_DecisionId_Returns_DecisionResponse()
        {
            const int expectedConcernsCaseUrn = 123;
            const int expectedDecisionId = 456;

            var testBuilder = new TestBuilder();

            var expectedDecisionResponse = testBuilder.Fixture.Build<GetDecisionResponse>()
                .With(x => x.DecisionId, expectedDecisionId)
                .Create();

            testBuilder.GetDecisionUseCase.Setup(x => x.Execute(It.Is<GetDecisionRequest>(r => r.ConcernsCaseUrn == expectedConcernsCaseUrn && r.DecisionId == expectedDecisionId), It.IsAny<CancellationToken>())).ReturnsAsync(expectedDecisionResponse);

            // Act
            var sut = testBuilder.BuildSut();
            var actionResult = await sut.GetById(expectedConcernsCaseUrn, expectedDecisionId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var expectedOkResult = new OkObjectResult(new ApiSingleResponseV2<GetDecisionResponse>(expectedDecisionResponse));
            okResult.Should().BeEquivalentTo(expectedOkResult);
        }

        [Fact]
        public async Task GetById_When_DecisionNotFound_Returns_NotFound()
        {
            const int expectedConcernsCaseUrn = 123;
            const int expectedDecisionId = 456;

            var testBuilder = new TestBuilder();

            testBuilder.GetDecisionUseCase.Setup(x => x.Execute(It.Is<GetDecisionRequest>(r => r.ConcernsCaseUrn == expectedConcernsCaseUrn && r.DecisionId == expectedDecisionId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(GetDecisionResponse));

            // Act
            var sut = testBuilder.BuildSut();
            var actionResult = await sut.GetById(expectedConcernsCaseUrn, expectedDecisionId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetDecisions_When_Invalid_Urn_Returns_BadRequest()
        {
            var testBuilder = new TestBuilder();

            var sut = testBuilder.BuildSut();

            var result = await sut.GetDecisions(0, CancellationToken.None);
            result.Result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task GetDecisions_When_Null_Response_Returns_NotFound()
        {
            var testBuilder = new TestBuilder();
            testBuilder.GetDecisionsUseCase
                .Setup(x => x.Execute(It.IsAny<GetDecisionsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(default(DecisionSummaryResponse[]));

            var sut = testBuilder.BuildSut();

            var result = await sut.GetDecisions(testBuilder.Fixture.Create<int>(), CancellationToken.None);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetDecisions_When_Decisions_Found_Returns_ApiSingleResponseV2_Of_DecisionSummaryArray()
        {

            var testBuilder = new TestBuilder();
            var expectedDtos = testBuilder.Fixture.CreateMany<DecisionSummaryResponse>(2).ToArray();

            testBuilder.GetDecisionsUseCase
                .Setup(x => x.Execute(It.IsAny<GetDecisionsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedDtos);

            var sut = testBuilder.BuildSut();

            var response = await sut.GetDecisions(testBuilder.Fixture.Create<int>(), CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var expectedOkResult = new OkObjectResult(new ApiSingleResponseV2<DecisionSummaryResponse[]>(expectedDtos));
            okResult.Should().BeEquivalentTo(expectedOkResult);
        }
        

        private class TestBuilder
        {
            internal Mock<ILogger<ConcernsCaseDecisionController>> MockLogger;
            internal Mock<IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse>> CreateDecisionUseCase;
            internal Mock<IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>> GetDecisionUseCase;
            internal Mock<IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>> GetDecisionsUseCase { get; set; }
            internal Fixture Fixture;

            public TestBuilder()
            {
                Fixture = new Fixture();
                MockLogger = new Mock<ILogger<ConcernsCaseDecisionController>>();
                CreateDecisionUseCase = new Mock<IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse>>();
                GetDecisionUseCase = new Mock<IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>>();
                GetDecisionsUseCase = new Mock<IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>>();
            }


            internal ConcernsCaseDecisionController BuildSut()
            {
                return new ConcernsCaseDecisionController(MockLogger.Object, CreateDecisionUseCase.Object, GetDecisionUseCase.Object, GetDecisionsUseCase.Object);
            }
        }
    }
}