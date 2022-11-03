using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Enums.Concerns;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.RequestModels.Concerns.Decisions
{
    public class CreateDecisionRequestTests
    {
        [Fact]
        public void IsValid_When_Invalid_DecisionType_Returns_False()
        {
            var fixture = new Fixture();
            var sut = fixture.Build<CreateDecisionRequest>()
                    .With(x => x.DecisionTypes, new DecisionType[] { 0 })
                    .Create();

            sut.IsValid().Should().BeFalse();
        }

        [Fact]
        public void IsValid_When_Valid_DecisionType_Returns_True()
        {
            var fixture = new Fixture();
            var sut = fixture.Build<CreateDecisionRequest>()
                .With(x => x.DecisionTypes, new DecisionType[] { DecisionType.EsfaApproval })
                .Create();

            sut.IsValid().Should().BeTrue();
        }

        [Fact]
        public void Constructor_Guards_Against_Nulls()
        {
            // Arrange
            var fixture = new Fixture();
            var assertion = fixture.Create<GuardClauseAssertion>();

            // Act & Assert
            assertion.Verify(typeof(CreateDecisionRequest).GetConstructors());
        }

        [Fact]
        public void Properties_Are_Initialized_By_Constructor()
        {
            // Arrange
            var fixture = new Fixture();
            var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();

            // Act & Assert
            assertion.Verify(typeof(CreateDecisionRequest));
        }

        [Fact]
        public void Property_Setters_Work_As_Expected()
        {
            // Arrange
            var fixture = new Fixture();
            var assertion = fixture.Create<WritablePropertyAssertion>();

            // Act & Assert
            assertion.Verify(typeof(CreateDecisionRequest));
        }
    }
}
