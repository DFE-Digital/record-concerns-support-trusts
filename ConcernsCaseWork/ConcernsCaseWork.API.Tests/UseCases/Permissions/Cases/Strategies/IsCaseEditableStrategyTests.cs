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
	public class IsCaseEditableStrategyTests
	{
		[Fact]
		public void Implements_ICaseActionPermissionStrategy()
		{
			var sut = new IsCaseEditableStrategy();
			sut.Should().BeAssignableTo<ICaseActionPermissionStrategy>();
		}

		[Fact]
		public void GetAllowedActionPermission_When_Null_Case_Returns_None()
		{
			var sut = new IsCaseEditableStrategy();
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

			var fakeCase = fixture
				.Build<ConcernsCase>()
				.With(x => x.ClosedAt, () => default)
				.Create();

			var sut = new IsCaseEditableStrategy();

			Action act = () => _ = sut.GetAllowedActionPermission(fakeCase, null);

			act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("userInfo");
		}

		[Fact]
		public void GetAllowedActionPermission_When_Case_Closed_Returns_None()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeUserInfo = fixture.Create<UserInfo>();
			var fakeCase = fixture
				.Build<ConcernsCase>()
				.With(x => x.ClosedAt, () => DateTime.UtcNow)
				.With(x => x.CreatedBy, () => fakeUserInfo.Name)
				.Create();

			var sut = new IsCaseEditableStrategy();

			var result = sut.GetAllowedActionPermission(fakeCase, fakeUserInfo);
			result.Should().Be(CasePermission.None);
		}

		[Fact]
		public void GetAllowedActionPermission_When_OpenCase_IsOwned_Returns_Edit()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeUserInfo = fixture.Create<UserInfo>();
			var fakeCase = fixture
				.Build<ConcernsCase>()
				.With(x => x.ClosedAt, () => default)
				.With(x => x.CreatedBy, () => fakeUserInfo.Name)
				.Create();

			var sut = new IsCaseEditableStrategy();

			var result = sut.GetAllowedActionPermission(fakeCase, fakeUserInfo);
			result.Should().Be(CasePermission.Edit);
		}

		[Theory]
		[InlineData(Claims.CaseWorkerRoleClaim)]
		[InlineData(Claims.TeamLeaderRoleClaim)]
		public void GetAllowedActionPermission_When_OpenCase_IsNotOwned_And_User_Is_NotAdmin_Returns_None(string role)
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeUserInfo = fixture.Build<UserInfo>()
				.With(x => x.Roles, () => new[] { role })
				.Create();

			var fakeCase = fixture
				.Build<ConcernsCase>()
				.With(x => x.ClosedAt, () => default)
				.With(x => x.CreatedBy, () => Guid.NewGuid().ToString())
				.Create();

			var sut = new IsCaseEditableStrategy();

			var result = sut.GetAllowedActionPermission(fakeCase, fakeUserInfo);
			result.Should().Be(CasePermission.None);
		}

		[Fact]
		public void GetAllowedActionPermission_When_OpenCase_IsNotOwned_But_User_Is_Admin_Returns_Edit()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var fakeUserInfo = fixture.Build<UserInfo>()
				.With(x => x.Roles, () => new[] { Claims.AdminRoleClaim })
				.Create();

			var fakeCase = fixture
				.Build<ConcernsCase>()
				.With(x => x.ClosedAt, () => default)
				.With(x => x.CreatedBy, () => Guid.NewGuid().ToString())
				.Create();

			var sut = new IsCaseEditableStrategy();

			var result = sut.GetAllowedActionPermission(fakeCase, fakeUserInfo);
			result.Should().Be(CasePermission.Edit);
		}

		[Fact]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(IsCaseEditableStrategy).GetConstructors());
		}
	}
}
