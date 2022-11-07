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
				.With(x => x.DecisionTypes, new Data.Enums.Concerns.DecisionType[] { 0 })
				.Create();

			sut.IsValid().Should().BeFalse();
		}
	}
}
