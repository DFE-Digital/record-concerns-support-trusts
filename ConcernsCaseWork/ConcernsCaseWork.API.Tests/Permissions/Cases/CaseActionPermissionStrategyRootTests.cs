using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Permissions.Cases
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
			var fakeCase = new ConcernsCaseResponse();
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
			var fakeCase = new ConcernsCaseResponse();
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
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(CaseActionPermissionStrategyRoot).GetMethods());
		}
	}
}
