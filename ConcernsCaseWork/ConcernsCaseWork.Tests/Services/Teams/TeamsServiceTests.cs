using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Service.Teams;
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
			Assert.IsInstanceOf<ITeamsModelService>(sut);
		}

		[Test]
		public async Task When_GetCaseworkTeam_Returns_Team_From_TramsApi()
		{
			var testFixture = new TestFixture()
				.WithPreviouslySelectedTeamMembers(new[] { "Fred.Flintstone", "Barney.Rubble" });

			var sut = testFixture.BuildSut();

			var result = await sut.GetCaseworkTeam(testFixture.CurrentUserName);

			Assert.IsNotNull(result);
			Assert.That(result.OwnerId, Is.EqualTo(testFixture.CurrentUserName));
			Assert.That(result.TeamMembers, Is.Not.Null);
			Assert.AreEqual(result.TeamMembers.Length, 2);
		}

		[Test]
		public async Task When_UpdateCaseworkTeam_And_SelectionEmpty_Then_RemovesSelectedUsers()
		{
			var testFixture = new TestFixture();

			var sut = testFixture.BuildSut();

			await sut.UpdateCaseworkTeam(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel(testFixture.CurrentUserName, Array.Empty<string>()));

			testFixture.MockCachedTeamsService.Verify(x => x.PutTeam(It.IsAny<ConcernsCaseworkTeamDto>()), Times.Once);
		}

		[Test]
		public async Task When_UpdateCaseworkTeam_And_SelectionsMade_Then_UpdatesSelectedUsers()
		{
			var testFixture = new TestFixture();

			var sut = testFixture
				.BuildSut();

			await sut.UpdateCaseworkTeam(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel(testFixture.CurrentUserName, new[] { "Fred.Flintstone" }));

			testFixture.MockCachedTeamsService.Verify(x => x.PutTeam(It.Is<ConcernsCaseworkTeamDto>(s => s.OwnerId == testFixture.CurrentUserName && s.TeamMembers[0] == "Fred.Flintstone")), Times.Once);
		}

		[Test]
		public void TeamsService_Methods_GuardAgainstNullArgs()
		{
			var fixture = new TestFixture().AutoFixture;
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsModelService).GetMethods());
		}

		[Test]
		public void TeamsService_Constructors_GuardAgainstNullArgs()
		{
			var fixture = new TestFixture().AutoFixture;
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsModelService).GetConstructors());
		}

		private class TestFixture
		{
			public Fixture AutoFixture;
			public Mock<ITeamsCachedService> MockCachedTeamsService;
			public Mock<ILogger<TeamsModelService>> MockLogger;

			public string CurrentUserName { get; }

			private Mapper _mapper;

			public TestFixture()
			{
				AutoFixture = new Fixture();
				MockCachedTeamsService = new Mock<ITeamsCachedService>();
				MockLogger = new Mock<ILogger<TeamsModelService>>();
				this.CurrentUserName = $"{AutoFixture.Create<string>()}.{AutoFixture.Create<string>()}";

				_mapper = this.CreateAutoMapper();
			}

			private Mapper CreateAutoMapper()
			{
				return new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>()));
			}

			public TestFixture WithPreviouslySelectedTeamMembers(params string[] teamMembers)
			{
				MockCachedTeamsService.Setup(x => x.GetTeam(this.CurrentUserName))
					.ReturnsAsync(new ConcernsCaseworkTeamDto(this.CurrentUserName, new[] { "Fred.Flintstone", "Barney.Rubble" }));

				return this;
			}

			public TeamsModelService BuildSut()
			{
				// never mock out automapper, let it be tested more by using the real mapping configurations
				return new TeamsModelService(MockLogger.Object, _mapper, MockCachedTeamsService.Object);
			}
		}
	}
}
