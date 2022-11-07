using AutoFixture;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.RequestModels.Concerns.Decisions
{
	public class UpdateDecisionRequestTests
	{
		[Fact]
		public void IsValid_When_Invalid_DecisionType_Returns_False()
		{
			var fixture = new Fixture();
			var sut = fixture.Build<UpdateDecisionRequest>()
				.With(x => x.DecisionTypes, new Data.Enums.Concerns.DecisionType[] { 0 })
				.Create();

			sut.IsValid().Should().BeFalse();
		}
	}
}
