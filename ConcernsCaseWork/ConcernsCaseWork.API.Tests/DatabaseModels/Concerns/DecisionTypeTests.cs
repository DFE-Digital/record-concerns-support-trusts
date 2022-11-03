using AutoFixture;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
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
        public void DecisionType_Properties_SetByConstructor(ConcernsCaseWork.Data.Enums.Concerns.DecisionType enumValue)
        {
            var fixture = new Fixture();
            var expectedId = enumValue;
            var expectedDecisionId = fixture.Create<int>();
            var sut = new DecisionType(expectedId) { DecisionId = expectedDecisionId };

            sut.DecisionTypeId.Should().Be(expectedId);
            sut.DecisionId.Should().Be(expectedDecisionId);

        }

        [Fact]
        public void Given_Invalid_DecisionTypes_Constructor_Throws_Exception()
        {
            Action action = () => new DecisionType(0) { DecisionId = 1 };

            action.Should().ThrowExactly<ArgumentOutOfRangeException>().And.ParamName.Should().Be("decisionType");
        }

        public static IEnumerable<object[]> EnumValues()
        {
            foreach (var number in Enum.GetValues(typeof(ConcernsCaseWork.Data.Enums.Concerns.DecisionType)))
            {
                yield return new object[] { number };
            }
        }
    }
}
