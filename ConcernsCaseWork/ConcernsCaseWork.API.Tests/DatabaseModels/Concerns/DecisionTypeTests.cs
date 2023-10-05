using AutoFixture;
using ConcernsCaseWork.Data.Models.Decisions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ConcernsCaseWork.API.Tests.DatabaseModels.Concerns
{
	public class DecisionTypeTests
	{
		[Fact]
		public void CanConstruct_DecisionType()
		{
			var fixture = new Fixture();
			var sut = fixture.Create<DecisionType>();
			sut.Should().NotBeNull();
		}

		[Theory]
		[MemberData(nameof(DecisionTypeTests.EnumValues))]
		public void DecisionType_Properties_SetByConstructor(Data.Enums.Concerns.DecisionType decisionTypeEnum)
		{
			var fixture = new Fixture();
			var expectedTypeId = decisionTypeEnum;
			var expectedFacilityAgreedId = API.Contracts.Decisions.DrawdownFacilityAgreed.No;
			var expectedCategoryId = API.Contracts.Decisions.FrameworkCategory.BuildingFinancialCapability;
			var expectedDecisionId = fixture.Create<int>();

			var sut = new DecisionType(expectedTypeId, expectedFacilityAgreedId, expectedCategoryId) { DecisionId = expectedDecisionId };

			sut.DecisionTypeId.Should().Be(expectedTypeId);
			sut.DecisionDrawdownFacilityAgreedId.Should().Be(expectedFacilityAgreedId);
			sut.DecisionFrameworkCategoryId.Should().Be(expectedCategoryId);
			sut.DecisionId.Should().Be(expectedDecisionId);
		}

		[Theory]
		[MemberData(nameof(DecisionTypeTests.EnumValues))]
		public void DecisionType_Properties_SetByConstructor_Accepts_Null(Data.Enums.Concerns.DecisionType decisionTypeEnum)
		{
			var fixture = new Fixture();
			var expectedTypeId = decisionTypeEnum;
			var expectedDecisionId = fixture.Create<int>();

			var sut = new DecisionType(expectedTypeId, null, null) { DecisionId = expectedDecisionId };

			sut.DecisionTypeId.Should().Be(expectedTypeId);
			sut.DecisionId.Should().Be(expectedDecisionId);
		}


		[Fact]
		public void Given_Invalid_DecisionTypes_Constructor_Throws_Exception()
		{
			Action action = () => new DecisionType(0, null, null) { DecisionId = 1 };

			action.Should().ThrowExactly<ArgumentOutOfRangeException>().And.ParamName.Should().Be("decisionType");
		}

		[Fact]
		public void Given_Invalid_DrawdownFacilityAgreed_Constructor_Throws_Exception()
		{
			Action action = () => new DecisionType(ConcernsCaseWork.Data.Enums.Concerns.DecisionType.EsfaApproval, 0, 0) { DecisionId = 1 };

			action.Should().ThrowExactly<ArgumentOutOfRangeException>().And.ParamName.Should().Be("decisionDrawdownFacilityAgreed");
		}

		[Fact]
		public void Given_Invalid_FrameworkCategory_Constructor_Throws_Exception()
		{
			Action action = () => new DecisionType(ConcernsCaseWork.Data.Enums.Concerns.DecisionType.EsfaApproval, ConcernsCaseWork.API.Contracts.Decisions.DrawdownFacilityAgreed.No, 0) { DecisionId = 1 };

			action.Should().ThrowExactly<ArgumentOutOfRangeException>().And.ParamName.Should().Be("decisionFrameworkCategory");
		}

		public static IEnumerable<object[]> EnumValues()
		{
			foreach (var number in Enum.GetValues(typeof(Data.Enums.Concerns.DecisionType)))
			{
				yield return new object[] { number };
			}
		}
	}
}
