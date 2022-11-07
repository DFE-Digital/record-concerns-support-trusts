using AutoFixture;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ConcernsCaseWork.API.Tests.DatabaseModels.Concerns
{
	public class DecisionStatusTests
	{
		[Fact]
		public void CanConstruct_DecisionStatus()
		{
			var fixture = new Fixture();
			var sut = fixture.Create<DecisionStatus>();
			sut.Should().NotBeNull();
		}

		[Theory]
		[MemberData(nameof(DecisionStatusTests.EnumValues))]
		public void DecisionStatus_Properties_SetByConstructor(Data.Enums.Concerns.DecisionStatus status)
		{
			var fixture = new Fixture();
			var expectedId = status;
			var expectedDescription = fixture.Create<string>();
			var sut = new DecisionStatus(expectedId) { Name = expectedDescription };

			sut.Id.Should().Be(expectedId);
			sut.Name.Should().Be(expectedDescription);
		}

		[Fact]
		public void Given_Invalid_DecisionStatus_Constructor_Throws_Exception()
		{
			Action action = () => new DecisionStatus(0) { Name = "not allowed" };

			action.Should().ThrowExactly<ArgumentOutOfRangeException>().And.ParamName.Should().Be("status");
		}

		public static IEnumerable<object[]> EnumValues()
		{
			foreach (var number in Enum.GetValues(typeof(Data.Enums.Concerns.DecisionStatus)))
			{
				yield return new object[] { number };
			}
		}
	}}