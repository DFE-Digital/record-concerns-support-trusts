using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Kernel;
using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Services.Teams;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
		public async Task When_GetTeamCaseworkSelectedUsers_Returns_Team_From_TramsApi()
		{
			var testFixture = new TestFixture()
				.WithPreviouslySelectedTeamMembers(new[] { "Fred.Flintstone", "Barney.Rubble" });

			var sut = testFixture.BuildSut();

			var result = await sut.GetTeamCaseworkSelectedUsers(testFixture.CurrentUserName);

			Assert.IsNotNull(result);
			Assert.That(result.OwnerId, Is.EqualTo(testFixture.CurrentUserName));
			Assert.That(result.TeamMembers, Is.Not.Null);
			Assert.AreEqual(result.TeamMembers.Length, 2);
		}

		[Test]
		public async Task When_UpdateTeamCaseworkSelectedUsers_And_SelectionEmpty_Then_RemovesSelectedUsers()
		{
			var testFixture = new TestFixture();

			var sut = testFixture.BuildSut();

			await sut.UpdateCaseworkTeam(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel(testFixture.CurrentUserName, Array.Empty<string>()));

			testFixture.MockTramsService.Verify(x => x.PutTeamCaseworkSelectedUsers(It.IsAny<Service.TRAMS.Teams.ConcernsCaseworkTeamDto>()), Times.Once);
		}

		[Test]
		public async Task When_UpdateTeamCaseworkSelectedUsers_And_SelectionsMade_Then_UpdatesSelectedUsers()
		{
			var testFixture = new TestFixture();

			var sut = testFixture
				.BuildSut();

			await sut.UpdateCaseworkTeam(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel(testFixture.CurrentUserName, new[] { "Fred.Flintstone" }));

			testFixture.MockTramsService.Verify(x => x.PutTeamCaseworkSelectedUsers(It.Is<Service.TRAMS.Teams.ConcernsCaseworkTeamDto>(s => s.OwnerId == testFixture.CurrentUserName && s.TeamMembers[0] == "Fred.Flintstone")), Times.Once);
		}

		[Test]
		public void TeamsService_Methods_GuardAgainstNullArgs()
		{
			var fixture = new TestFixture().AutoFixture;
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsService).GetMethods());
		}

		[Test]
		public void TeamsService_Constructors_GuardAgainstNullArgs()
		{
			var fixture = new TestFixture().AutoFixture;
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsService).GetConstructors());
		}

		private class TestFixture
		{
			public Fixture AutoFixture;
			public Mock<Service.TRAMS.Teams.ITeamsService> MockTramsService;
			public Mock<ILogger<TeamsService>> MockLogger;

			public string CurrentUserName { get; }

			private Mapper _mapper;

			public TestFixture()
			{
				AutoFixture = new Fixture();
				MockTramsService = new Mock<Service.TRAMS.Teams.ITeamsService>();
				MockLogger = new Mock<ILogger<TeamsService>>();
				this.CurrentUserName = $"{AutoFixture.Create<string>()}.{AutoFixture.Create<string>()}";

				_mapper = this.CreateAutoMapper();
			}

			private Mapper CreateAutoMapper()
			{
				return new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>()));
			}

			public TestFixture WithPreviouslySelectedTeamMembers(params string[] teamMembers)
			{
				MockTramsService.Setup(x => x.GetTeamCaseworkSelectedUsers(this.CurrentUserName))
					.ReturnsAsync(new Service.TRAMS.Teams.ConcernsCaseworkTeamDto(this.CurrentUserName, new[] { "Fred.Flintstone", "Barney.Rubble" }));

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
