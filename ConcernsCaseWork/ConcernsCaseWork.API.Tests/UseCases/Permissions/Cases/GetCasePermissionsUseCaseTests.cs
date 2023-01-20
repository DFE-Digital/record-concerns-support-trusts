using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions.Cases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.Permissions.Cases
{
	public class GetCasePermissionsUseCaseTests
	{
		[Fact]
		public void GetCasePermissionsUserCase_Implements_IGetCasePermissionsUserCase()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());

			var sut = fixture.Create<GetCasePermissionsUseCase>();
			sut.Should().BeAssignableTo<IGetCasePermissionsUseCase>();
		}

		[Fact]
		public async Task When_Execute_With_Multiple_Cases_Returns_Results_For_Each_Case()
		{
			// arrange
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			var caseIds = new long[] { 1, 2, 3 };
			var userInfo = new UserInfo() { Name = "John.Smith@Dfe.gov.uk", Roles = new[] { Claims.CaseWorkerRoleClaim } };

			var mockRootStrategy = fixture.Freeze<Mock<ICaseActionPermissionStrategyRoot>>();
			mockRootStrategy.Setup(x => x.GetPermittedCaseActions(It.IsAny<ConcernsCase>(), userInfo)).Returns(() => new[] { CasePermission.View });

			var mockGateway = fixture.Freeze<Mock<IConcernsCaseGateway>>();
			foreach (long caseId in caseIds)
			{
				mockGateway.Setup(x => x.GetConcernsCaseIncludingRecordsById((int)caseId)).Returns(() =>
					fixture.Build<ConcernsCase>()
						.With(x => x.Id, caseId)
						.Create()
				);
			}

			// act
			var sut = fixture.Create<GetCasePermissionsUseCase>();
			var results = await sut.Execute((caseIds, userInfo), CancellationToken.None);

			// assert
			results.CasePermissionResponses.Length.Should().Be(3);
			for (int i = 0; i < caseIds.Length; i++)
			{
				results.CasePermissionResponses[i].CaseId.Should().Be(caseIds[i]);
				results.CasePermissionResponses[i].Permissions.Length.Should().Be(1);
				results.CasePermissionResponses[i].Permissions[0].Should().Be(CasePermission.View);
			}
		}

		[Fact]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(GetCasePermissionsUseCase).GetConstructors());
		}

		[Fact]
		public void Methods_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(GetCasePermissionsUseCase).GetMethods());
		}
	}
}
