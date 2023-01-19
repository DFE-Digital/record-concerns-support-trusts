using AutoFixture;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
	public class UpdateDecisionResponseFactoryTests
	{
		[Fact]
		public void Can_Create_Response()
		{
			var fixture = CreateFixture();

			var expectedConcernsCaseUrn = fixture.Create<int>();
			var expectedDecisionId = fixture.Create<int>();

			var sut = new UpdateDecisionResponseFactory();
			var result = sut.Create(expectedConcernsCaseUrn, expectedDecisionId);

			result.ConcernsCaseUrn.Should().Be(expectedConcernsCaseUrn);
			result.DecisionId.Should().Be(expectedDecisionId);
		}

		private static Fixture CreateFixture()
		{
			var fixture = new Fixture();
			fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			return fixture;
		}
	}
}