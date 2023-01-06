using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
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
				.With(x => x.DecisionTypes, new DecisionType[] { 0 })
				.Create();

			sut.IsValid().Should().BeFalse();
		}
	}
}
