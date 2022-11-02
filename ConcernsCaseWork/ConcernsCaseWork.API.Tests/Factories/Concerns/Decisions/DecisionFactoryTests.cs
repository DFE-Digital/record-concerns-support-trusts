using AutoFixture;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
    public class DecisionFactoryTests
    {
        [Fact]
        public void DecisionFactory_Create_CreatesDecision()
        {
            var fixture = new Fixture();

            var concernsCaseId = fixture.Create<int>();
            var input = fixture.Create<CreateDecisionRequest>();

            var sut = new DecisionFactory();
            var decision = sut.CreateDecision(concernsCaseId, input);

            decision.Should().BeEquivalentTo(input, cfg => cfg.Excluding(x => x.DecisionTypes)
                .Excluding(x => x.ConcernsCaseUrn));

            for (int i = 0; i < input.DecisionTypes.Length; i++)
            {
                decision.DecisionTypes[i].DecisionTypeId.Should().Be(input.DecisionTypes[i]);
            }
        }
    }
}
