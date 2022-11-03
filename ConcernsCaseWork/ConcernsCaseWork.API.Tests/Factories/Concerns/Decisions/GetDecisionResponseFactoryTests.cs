using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
    public class GetDecisionResponseFactoryTests
    {
        [Fact]
        public void Can_Construct_GetDecisionResponseFactory()
        {
            var fixture = CreateFixture();
            var sut = fixture.Create<GetDecisionResponseFactory>();
            sut.Should().NotBeNull();
        }

        [Fact]
        public void GetDecisionResponseFactory_Is_A_IGetDecisionResponseFactory()
        {
            var fixture = CreateFixture();
            var sut = fixture.Create<GetDecisionResponseFactory>();
            sut.Should().BeAssignableTo<IGetDecisionResponseFactory>();
        }

        [Fact]
        public void Create_Returns_DecisionResponse()
        {
            var fixture = CreateFixture();

            var concernsCaseUrn = fixture.Create<int>();
            var decision = fixture.Create<Decision>();

            var sut = new GetDecisionResponseFactory();

            var result = sut.Create(concernsCaseUrn, decision);

            result.ConcernsCaseUrn.Should().Be(concernsCaseUrn);
            result.Should().BeEquivalentTo(decision,
                opt => opt.IncludingAllDeclaredProperties()
                    .Excluding(x => x.DecisionTypes)
                    .Excluding(x => x.Status));
            result.DecisionTypes.Should().BeEquivalentTo(decision.DecisionTypes.Select(x => x.DecisionTypeId),
                opt => opt.WithStrictOrdering());
            result.DecisionStatus.Should().Be(decision.Status);
        }

        [Fact]
        public void Constructor_Guards_Against_Null_Arguments()
        {
            // Arrange
            var fixture = CreateFixture();
            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(GetDecisionResponseFactory).GetConstructors());
        }

        [Fact]
        public void Methods_Guard_Against_Null_Arguments()
        {
            // Arrange
            var fixture = CreateFixture();
            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(GetDecisionResponseFactory).GetMethods());
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
    }
}
