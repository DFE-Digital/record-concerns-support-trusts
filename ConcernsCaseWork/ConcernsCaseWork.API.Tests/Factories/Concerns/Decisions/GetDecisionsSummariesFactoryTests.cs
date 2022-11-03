﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
    public class GetDecisionsSummariesFactoryTests
    {
        [Fact]
        public void GetDecisionsSummariesFactory_Implements_IGetDecisionsSummariesFactory()
        {
            var sut = new GetDecisionsSummariesFactory();
            sut.Should().BeAssignableTo<IGetDecisionsSummariesFactory>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_When_Invalid_Urn_Throws_Exception(int invalidUrn)
        {
            var fixture = CreateFixture();
            
            var expectedDecisions = CreateDecisions(fixture, fixture.Create<int>());

            var sut = new GetDecisionsSummariesFactory();
            Action act = () => sut.Create(invalidUrn, expectedDecisions);
            act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("concernsCaseUrn");
        }

        [Fact]
        public void Create_When_Null_Decisions_Throws_Exception()
        {
            var fixture = CreateFixture();

            var sut = new GetDecisionsSummariesFactory();
            Action act = () => sut.Create(fixture.Create<int>(), null);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("decisions");
        }

        [Fact]
        public void Create_Maps_Decisions_To_DecisionSummaries()
        {
            var fixture = CreateFixture();
            
            var expectedDecisions = CreateDecisions(fixture, fixture.Create<int>());

            var sut = new GetDecisionsSummariesFactory();
            var result = sut.Create(123, expectedDecisions);

            expectedDecisions.Should().BeEquivalentTo(result, opt =>
                opt.IncludingAllDeclaredProperties()
                    .Excluding(x => x.ConcernsCaseUrn)
                    .Excluding(x => x.Title)
                );
        }

        [Fact]
        public void Create_When_Decision_Has_No_DecisionTypes_Maps_Decisions_To_DecisionSummaries()
        {
            var fixture = CreateFixture();

            var expectedDecisions = CreateDecisions(fixture, fixture.Create<int>(), false);

            var sut = new GetDecisionsSummariesFactory();
            var result = sut.Create(123, expectedDecisions);

            expectedDecisions.Should().BeEquivalentTo(result, opt => 
            opt.IncludingAllDeclaredProperties()
                .Excluding(x => x.ConcernsCaseUrn)
                .Excluding(x => x.Title));
            result[0].Title.Should().Be("Not Available");
        }

        private Fixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }

        private Decision[] CreateDecisions(Fixture fixture, int count, bool includeDecisionTypes = true)
        {
            List<Decision> decisions = new List<Decision>();
            for (int i = 0; i < count; i++)
            {
                var decision = Decision.CreateNew(
                    concernsCaseId: fixture.Create<int>(),
                    crmCaseNumber: new string(fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
                    retrospectiveApproval: fixture.Create<bool>(),
                    submissionRequired: fixture.Create<bool>(),
                    submissionDocumentLink: new string(fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
                    receivedRequestDate: DateTimeOffset.Now,
                    decisionTypes: includeDecisionTypes ? new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove)} : null,
                    totalAmountRequested: fixture.Create<decimal>(),
                    supportingNotes: new string(fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
                    createdAt: DateTimeOffset.Now
                );
                decision.DecisionId = fixture.Create<int>();
                decisions.Add(decision);
            }

            return decisions.ToArray();
        }
        
    }
}
