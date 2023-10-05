﻿using AutoFixture;
using ConcernsCaseWork.Data.Models.Decisions;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.DatabaseModels.Concerns
{
	public class DecisionTypeIdTests
	{
		[Fact]
		public void CanConstruct_DecisionType()
		{
			var fixture = new Fixture();
			var sut = fixture.Create<DecisionTypeId>();
			sut.Should().NotBeNull();
		}

		[Fact]
		public void DecisionType_Properties_SetByConstructor()
		{
			var fixture = new Fixture();
			const string noticeToImproveNti = "Notice to Improve (NTI)";
			var expectedId = Data.Enums.Concerns.DecisionType.EsfaApproval;
			var sut = new DecisionTypeId(expectedId, noticeToImproveNti);

			sut.Id.Should().Be(expectedId);
			sut.Name.Should().Be(noticeToImproveNti);
		}
	}
}
