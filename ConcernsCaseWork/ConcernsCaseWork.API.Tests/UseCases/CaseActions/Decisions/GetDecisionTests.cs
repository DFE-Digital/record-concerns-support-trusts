using AutoFixture;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.Decisions
{
    public class GetDecisionTests
    {
        [Fact]
        public void GetDecision_Implements_IUseCaseAsync()
        {
            typeof(GetDecision).Should().BeAssignableTo<IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>>();
        }

        [Fact]
        public async Task Execute_Throws_Exception_If_Request_IsNull()
        {
            var sut = new GetDecision(Mock.Of<IConcernsCaseGateway>(), Mock.Of<IGetDecisionResponseFactory>());

            Func<Task> action = async () => await sut.Execute(null, CancellationToken.None);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Execute_When_No_Decision_Returns_Null()
        {
            var fixture = CreateFixture();
            var sut = new GetDecision(Mock.Of<IConcernsCaseGateway>(), Mock.Of<IGetDecisionResponseFactory>());

            var request = fixture.Create<GetDecisionRequest>();
            var result = await sut.Execute(request, CancellationToken.None);

            result.Should().BeNull();
        }

        private Fixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize<Decision>(sb => sb.FromFactory(() =>
            {
                var decision = Decision.CreateNew(
                    concernsCaseId: fixture.Create<int>(),
                    crmCaseNumber: new string(fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
                    retrospectiveApproval: fixture.Create<bool>(),
                    submissionRequired: fixture.Create<bool>(),
                    submissionDocumentLink: new string(fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
                    receivedRequestDate: DateTimeOffset.Now,
                    decisionTypes: new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove) },
                    totalAmountRequested: fixture.Create<decimal>(),
                    supportingNotes: new string(fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
                    createdAt: DateTimeOffset.Now
                );
                decision.DecisionId = fixture.Create<int>();
                return decision;
            }));

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }

        [Fact]
        public async Task Execute_When_Decision_Found_Returns_GetDecisionResponse()
        {
            // arrange
            var fixture = CreateFixture();
            var fakeDecision = fixture.Create<Decision>();
            var fakeConcernsCase = CreateRandomConcernsCase(fixture, fakeDecision);

            var fakeRequest = new GetDecisionRequest(fakeConcernsCase.Urn, fakeDecision.DecisionId);

            var mockGateWay = new Mock<IConcernsCaseGateway>();
            mockGateWay
                .Setup(x => x.GetConcernsCaseByUrn(fakeRequest.ConcernsCaseUrn))
                .Returns(fakeConcernsCase);

            var fakeResponse = fixture.Build<GetDecisionResponse>()
                .With(x => x.DecisionId, fakeRequest.DecisionId)
                .With(x => x.ConcernsCaseId, fakeConcernsCase.Id)
                .Create();

            var mockGetDecisionResponseFactory = new Mock<IGetDecisionResponseFactory>();
            mockGetDecisionResponseFactory
                .Setup(x => x.Create(fakeConcernsCase.Urn, fakeDecision))
                .Returns(fakeResponse);

            // act
            var sut = new GetDecision(mockGateWay.Object, mockGetDecisionResponseFactory.Object);
            var response = await sut.Execute(fakeRequest, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.ConcernsCaseId.Should().Be(fakeConcernsCase.Id);
            response.DecisionId.Should().Be(fakeRequest.DecisionId);
        }

        private ConcernsCase CreateRandomConcernsCase(Fixture fixture, params Decision[] decisions)
        {
            if (decisions is null)
            {
                decisions = Array.Empty<Decision>();
            }

            var newCase = fixture.Create<ConcernsCase>();
            foreach (var decision in decisions)
            {
                newCase.AddDecision(decision);
            }

            return newCase;
        }
    }
}