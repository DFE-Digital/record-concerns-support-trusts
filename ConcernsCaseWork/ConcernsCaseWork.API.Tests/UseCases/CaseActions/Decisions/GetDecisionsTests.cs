using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.Decisions
{
    public class GetDecisionsTests
    {
        [Fact]
        public void GetDecisions_Implements_IUseCaseAsync()
        {
            var fixture = CreateFixture();
            var sut = fixture.Create<GetDecisions>();
            sut.Should().BeAssignableTo<IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>>();
        }

        [Fact]
        public void Constructor_Guards_Against_Null_Arguments()
        {
            // Arrange
            var fixture = CreateFixture();

            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(GetDecisions).GetConstructors());
        }

        [Fact]
        public void Methods_Guards_Against_Null_Arguments()
        {
            // Arrange
            var fixture = CreateFixture();

            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(GetDecisions).GetMethods());
        }

        [Fact]
        public async Task Execute_Returns_Null_When_No_ConcernsCase_Found()
        {
            var mockGateWay = new Mock<IConcernsCaseGateway>();
            var mockLogger = new Mock<ILogger<GetDecisions>>();
            var fixture = CreateFixture(mockLogger.Object, mockGateWay.Object);
            var expectedRequest = fixture.Create<GetDecisionsRequest>();

            mockGateWay.Setup(x => x.GetConcernsCaseByUrn(It.IsAny<int>(), false)).Returns(default(ConcernsCase));

            var sut = fixture.Create<GetDecisions>();
            var result = await sut.Execute(expectedRequest, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Execute_Returns_Empty_Array_When_ConcernsCase_Has_No_Decisions()
        {
            var mockGateWay = new Mock<IConcernsCaseGateway>();
            var mockLogger = new Mock<ILogger<GetDecisions>>();
            var mockFactory = new Mock<IGetDecisionsSummariesFactory>();

            var fixture = CreateFixture(mockLogger.Object, mockGateWay.Object, mockFactory.Object);

            var expectedRequest = fixture.Create<GetDecisionsRequest>();
            var expectedConcernsCase = fixture.Create<ConcernsCase>();

            mockGateWay.Setup(x => x.GetConcernsCaseByUrn(It.IsAny<int>(), false)).Returns(expectedConcernsCase);

            var sut = fixture.Create<GetDecisions>();
            var result = await sut.Execute(expectedRequest, CancellationToken.None);

            result.Should().BeEquivalentTo(Array.Empty<DecisionSummaryResponse>());
            mockFactory.Verify(x => x.Create(expectedRequest.ConcernsCaseUrn, It.IsAny<IEnumerable<Decision>>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Returns_Summary_Array_When_ConcernsCase_Has_Decisions()
        {
            var mockGateWay = new Mock<IConcernsCaseGateway>();
            var mockLogger = new Mock<ILogger<GetDecisions>>();
            var mockFactory = new Mock<IGetDecisionsSummariesFactory>();

            var fixture = CreateFixture(mockLogger.Object, mockGateWay.Object, mockFactory.Object);

            var expectedConcernsCase = fixture.Create<ConcernsCase>();
            var expectedRequest = new GetDecisionsRequest(expectedConcernsCase.Urn);

            expectedConcernsCase.AddDecision(fixture.Create<Decision>(), DateTimeOffset.Now);

            var expectedResult = new[]
            {
                new DecisionSummaryResponse() { ConcernsCaseUrn = expectedConcernsCase.Urn }
            };

            mockGateWay.Setup(x => x.GetConcernsCaseByUrn(It.IsAny<int>(), false))
                .Returns(expectedConcernsCase);

            mockFactory.Setup(x => x.Create(expectedConcernsCase.Urn, It.IsAny<IEnumerable<Decision>>()))
                .Returns(() => expectedResult);

            var sut = fixture.Create<GetDecisions>();
            var result = await sut.Execute(expectedRequest, CancellationToken.None);

            mockFactory.Verify(x => x.Create(expectedConcernsCase.Urn, It.IsAny<IEnumerable<Decision>>()), Times.Once);
            result.Should().BeSameAs(expectedResult);
        }

        private static Fixture CreateFixture(
            ILogger<GetDecisions> logger = null,
            IConcernsCaseGateway gateway = null,
            IGetDecisionsSummariesFactory decisionsSummariesFactory = null)
        {
            var fixture = new Fixture();
            fixture.Register(() => logger ?? Mock.Of<ILogger<GetDecisions>>());
            fixture.Register(() => gateway ?? Mock.Of<IConcernsCaseGateway>());
            fixture.Register(() => decisionsSummariesFactory ?? Mock.Of<IGetDecisionsSummariesFactory>());

            fixture.Customize<Decision>(sb => sb.FromFactory(() =>
            {
                var decision = Decision.CreateNew(
                    crmCaseNumber: new string(fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
                    retrospectiveApproval: fixture.Create<bool>(),
                    submissionRequired: fixture.Create<bool>(),
                    submissionDocumentLink: new string(fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
                    receivedRequestDate: DateTimeOffset.Now,
                    decisionTypes: new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove) },
                    totalAmountRequested: fixture.Create<decimal>(),
                    supportingNotes: new string(fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
                    DateTimeOffset.Now
                );
                decision.DecisionId = fixture.Create<int>();
                return decision;
            }));

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
