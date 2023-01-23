using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.Permissions.Cases.Strategies
{
	public class IsCaseViewableStrategyTests
	{
		[Fact]
		public void Implements_ICaseActionPermissionStrategy()
		{
			var sut = new IsCaseViewableStrategy();
			sut.Should().BeAssignableTo<ICaseActionPermissionStrategy>();
		}

		[Fact]
		public void GetAllowedActionPermission_When_Null_Case_Returns_None()
		{
			var sut = new IsCaseViewableStrategy();
			var result = sut.GetAllowedActionPermission(null, new UserInfo());

			result.Should().Be(CasePermission.None);
		}

		[Fact]
		public void GetAllowedActionPermission_Guards_Against_Null_UserInfo()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeCase = fixture.Create<ConcernsCase>();

			var sut = new IsCaseViewableStrategy();

			Action act = () => _ = sut.GetAllowedActionPermission(fakeCase, null);

			act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("userInfo");
		}

		[Fact]
		public void GetAllowedActionPermission_When_Args_Valid_Returns_View()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeCase = fixture.Create<ConcernsCase>();

			var sut = new IsCaseViewableStrategy();
			var result = sut.GetAllowedActionPermission(fakeCase, new UserInfo());

			result.Should().Be(CasePermission.View);
		}

		[Fact]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(IsCaseViewableStrategy).GetConstructors());
		}
	}
}
