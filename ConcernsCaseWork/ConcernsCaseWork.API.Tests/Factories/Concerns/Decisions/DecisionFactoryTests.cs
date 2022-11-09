using AutoFixture;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
	public class DecisionFactoryTests
	{
		[Fact]
		public void DecisionFactory_Create_With_CreateDecisionRequest_CreatesDecision()
		{
			var fixture = new Fixture();

			var input = fixture.Create<CreateDecisionRequest>();

			var sut = new DecisionFactory();
			var decision = sut.CreateDecision(input);

			decision.Should().BeEquivalentTo(input,
				cfg => cfg.Excluding(x => x.DecisionTypes)
					.Excluding(x => x.ConcernsCaseUrn));

			decision.DecisionTypes.Select(x => (int)x.DecisionTypeId)
				.Should().Contain(input.DecisionTypes.Select(i => (int)i));
		}

		[Fact]
		public void DecisionFactory_Create_With_UpdateDecisionRequest_CreatesDecision()
		{
			var fixture = new Fixture();

			var input = fixture.Create<UpdateDecisionRequest>();

			var sut = new DecisionFactory();
			var decision = sut.CreateDecision(input);

			decision.Should().BeEquivalentTo(input,
				cfg => cfg.Excluding(x => x.DecisionTypes));

			decision.DecisionTypes.Select(x => (int)x.DecisionTypeId)
				.Should().Contain(input.DecisionTypes.Select(i => (int)i));
		}
	}
}
