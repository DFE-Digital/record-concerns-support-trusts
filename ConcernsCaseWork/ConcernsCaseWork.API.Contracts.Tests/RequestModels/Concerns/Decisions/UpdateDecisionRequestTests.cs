using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions;
using FluentAssertions;

namespace ConcernsCaseWork.API.Tests.RequestModels.Concerns.Decisions
{
	public class UpdateDecisionRequestTests
	{
		[Fact]
		public void IsValid_When_Invalid_DecisionType_Returns_False()
		{
			var fixture = new Fixture();
			var sut = fixture.Build<UpdateDecisionRequest>()
				.With(x => x.DecisionTypes, new DecisionTypeQuestion[] { new DecisionTypeQuestion() { Id = 0 } })
				.Create();

			sut.IsValid().Should().BeFalse();
		}
	}
}
