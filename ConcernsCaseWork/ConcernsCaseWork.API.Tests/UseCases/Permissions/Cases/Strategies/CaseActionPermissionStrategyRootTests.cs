using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.Permissions.Cases.Strategies
{
	public class CaseActionPermissionStrategyRootTests
	{
		[Fact]
		public void Implements_ICaseActionPermissionStrategyRoot()
		{
			var sut = new CaseActionPermissionStrategyRoot(Array.Empty<ICaseActionPermissionStrategy>());
			sut.Should().BeAssignableTo<ICaseActionPermissionStrategyRoot>();
		}

		[Fact]
		public void GetPermittedCaseActions_Calls_All_Strategies()
		{
			var fakeCase = new ConcernsCase();
			var fakeUser = new UserInfo();

			var mockStrategies = new ICaseActionPermissionStrategy[]
			{
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.None),
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.None),
			};

			var sut = new CaseActionPermissionStrategyRoot(mockStrategies);

			sut.GetPermittedCaseActions(fakeCase, fakeUser);

			Mock.Get(mockStrategies[0]).Verify(x => x.GetAllowedActionPermission(fakeCase, fakeUser), Times.Once);
			Mock.Get(mockStrategies[1]).Verify(x => x.GetAllowedActionPermission(fakeCase, fakeUser), Times.Once);
		}

		[Fact]
		public void GetPermittedCaseActions_Returns_Only_Positive_Results()
		{
			var fakeCase = new ConcernsCase();
			var fakeUser = new UserInfo();

			var mockStrategies = new ICaseActionPermissionStrategy[]
			{
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.None),
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.Edit),
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.None),
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.View),
			};

			var sut = new CaseActionPermissionStrategyRoot(mockStrategies);

			var results = sut.GetPermittedCaseActions(fakeCase, fakeUser);

			results.Length.Should().Be(2);
			results.All(x => x != CasePermission.None);
		}

		[Fact]
		public void GetPermittedCaseActions_If_Duplicate_Permissions_Detected_Throws_Exception()
		{
			var fakeCase = new ConcernsCase();
			var fakeUser = new UserInfo();

			var mockStrategies = new ICaseActionPermissionStrategy[]
			{
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.View),
				Mock.Of<ICaseActionPermissionStrategy>(x => x.GetAllowedActionPermission(fakeCase, fakeUser) == CasePermission.View),
			};

			var sut = new CaseActionPermissionStrategyRoot(mockStrategies);

			Action act = () => sut.GetPermittedCaseActions(fakeCase, fakeUser);

			act.Should().Throw<InvalidOperationException>().And.Message.Should().Be("One or more strategies returned a duplicate permission. Check that strategies are not being called multiple times and that strategies return unique permissions.");
		}

		[Fact]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(CaseActionPermissionStrategyRoot).GetConstructors());
		}

		[Fact]
		public void Methods_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(CaseActionPermissionStrategyRoot).GetMethods());
		}
	}
}
