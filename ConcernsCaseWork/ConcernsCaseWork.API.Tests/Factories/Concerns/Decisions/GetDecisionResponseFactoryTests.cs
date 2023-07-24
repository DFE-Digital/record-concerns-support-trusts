﻿using AutoFixture;
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
					.Excluding(x => x.Status)
					.Excluding(x => x.ConcernsCaseId)
					.Excluding(x => x.Outcome)
					.Excluding(x => x.DeletedAt)
				);

			result.Outcome.Status.Should().Be(decision.Outcome.Status);
			result.Outcome.Authorizer.Should().Be(decision.Outcome.Authorizer);
			result.Outcome.DecisionMadeDate.Should().Be(decision.Outcome.DecisionMadeDate);
			result.Outcome.DecisionEffectiveFromDate.Should().Be(decision.Outcome.DecisionEffectiveFromDate);
			result.Outcome.TotalAmount.Should().Be(decision.Outcome.TotalAmount);

			var expectedBusinessAreas = decision.Outcome.BusinessAreasConsulted.Select(b => b.DecisionOutcomeBusinessId);

			result.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(expectedBusinessAreas);

            result.DecisionTypes.Select(t => t.Id).Should().BeEquivalentTo(decision.DecisionTypes.Select(x => x.DecisionTypeId),
                opt => opt.WithStrictOrdering());
            result.DecisionStatus.Should().HaveSameValueAs(decision.Status);
            result.Title.Should().Be(decision.GetTitle());
        }
        
        [Fact]
        public void Create_WhenClosedAtIsNull_Returns_DecisionResponse_WithIsEditable_True()
        {
	        var fixture = CreateFixture();

	        var concernsCaseUrn = fixture.Create<int>();
	        var decision = fixture.Create<Decision>();
	        decision.ClosedAt = null;

	        var sut = new GetDecisionResponseFactory();

	        var result = sut.Create(concernsCaseUrn, decision);

	        result.IsEditable.Should().BeTrue();
        }
        
        [Fact]
        public void Create_WhenClosedAtIsNotNull_Returns_DecisionResponse_WithIsEditable_False()
        {
	        var fixture = CreateFixture();

	        var concernsCaseUrn = fixture.Create<int>();
	        var decision = fixture.Create<Decision>();

	        var sut = new GetDecisionResponseFactory();

	        var result = sut.Create(concernsCaseUrn, decision);

	        result.IsEditable.Should().BeFalse();
        }
        
		[Fact]
		public void Create_WithNoOutcome_Returns_DecisionResponse()
		{
			var fixture = CreateFixture();

			var concernsCaseUrn = fixture.Create<int>();
			var decision = fixture.Create<Decision>();
			decision.Outcome = null;

			var sut = new GetDecisionResponseFactory();

			var result = sut.Create(concernsCaseUrn, decision);

			result.Outcome.Should().BeNull();
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
                    crmCaseNumber: new string(fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
                    retrospectiveApproval: fixture.Create<bool>(),
                    submissionRequired: fixture.Create<bool>(),
                    submissionDocumentLink: new string(fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
                    receivedRequestDate: DateTimeOffset.Now,
                    // TODO EA this as well
                    decisionTypes: new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove, null, null) },
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
    }}
