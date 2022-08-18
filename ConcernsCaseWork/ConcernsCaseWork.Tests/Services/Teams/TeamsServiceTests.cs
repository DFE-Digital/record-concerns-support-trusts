using AutoFixture;
using AutoMapper;
using ConcernsCaseWork.Mappers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Teams;
using System.Threading.Tasks;
using ITeamsService = ConcernsCaseWork.Services.Teams.ITeamsService;
using TeamsService = ConcernsCaseWork.Services.Teams.TeamsService;

namespace ConcernsCaseWork.Tests.Services.Teams
{
	[Parallelizable(ParallelScope.All)]
	public class TeamsServiceTests
	{
		[Test]
		public void TeamsService_Implements_ITeamsService()
		{
			var sut = new TestFixture().BuildSut();
			Assert.IsInstanceOf<ITeamsService>(sut);
		}

		[Test]
		public async Task When_GetTeamCaseworkSelectedUsers_Returns_UsersArray_From_TramsApi()
		{
			var testFixture = new TestFixture()
				.WithSelectedTeamMembers(new[] { "Fred.Flintstone", "Barney.Rubble" });

			var sut = testFixture.BuildSut();

			var result = await sut.GetTeamCaseworkSelectedUsers(testFixture.CurrentUserName);

			Assert.IsNotNull(result);
			Assert.That(result.UserName, Is.EqualTo(testFixture.CurrentUserName));
			Assert.That(result.SelectedTeamMembers, Is.Not.Null);
			Assert.AreEqual(result.SelectedTeamMembers.Length, 2);
		}

		[Test]
		public async Task When_UpdateTeamCaseworkSelectedUsers_And_SelectionEmpty_Then_RemovesSelectedUsers()
		{
			var testFixture = new TestFixture()
				.WithSelectedTeamMembers(new[] { "Fred.Flintstone", "Barney.Rubble" });

			var sut = testFixture.BuildSut();

			//var results = await sut.UpdateTeamCaseworkSelectedUsers
		}

		private class TestFixture
		{
			private Fixture _fixture;
			public Mock<Service.TRAMS.Teams.ITeamsService> MockTramsService;
			public Mock<ILogger<TeamsService>> MockLogger;

			public string CurrentUserName { get; }

			private Mapper _mapper;

			public TestFixture()
			{
				_fixture = new Fixture();
				MockTramsService = new Mock<Service.TRAMS.Teams.ITeamsService>();
				MockLogger = new Mock<ILogger<TeamsService>>();
				this.CurrentUserName = $"{_fixture.Create<string>()}.{_fixture.Create<string>()}";

				_mapper = this.CreateAutoMapper();
			}

			private Mapper CreateAutoMapper()
			{
				return new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>()));
			}

			public TestFixture WithSelectedTeamMembers(params string[] teamMembers)
			{
				MockTramsService.Setup(x => x.GetTeamCaseworkSelectedUsers(this.CurrentUserName))
					.ReturnsAsync(new TeamCaseworkUsersSelectionDto(this.CurrentUserName, new[] { "Fred.Flintstone", "Barney.Rubble" }));

				return this;
			}

			public TeamsService BuildSut()
			{
				// never mock out automapper, let it be tested more by using the real mapping configurations
				return new TeamsService(MockLogger.Object, _mapper, MockTramsService.Object);
			}
		}
	}
}
