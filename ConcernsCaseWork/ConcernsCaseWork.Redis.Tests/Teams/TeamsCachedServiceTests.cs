using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Service.Teams;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.Teams
{
	[Parallelizable(ParallelScope.All)]
	public class TeamsCachedServiceTests
	{
		[Test]
		public void TeamsCachedService_Decorates_ITeamsService()
		{
			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), Mock.Of<ITeamsService>(), Mock.Of<ICacheProvider>());
			Assert.That(sut, Is.AssignableTo<ITeamsService>());
			Assert.That(sut, Is.AssignableTo<ITeamsCachedService>());
		}

		[Test]
		public void Methods_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsCachedService).GetMethods());
		}

		[Test]
		public void Constructors_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsCachedService).GetConstructors());
		}

		[Test]
		public async Task GetTeam_When_NotCached_Returns_Data_From_TeamsService()
		{
			// arrange
			var expectedTeamOwner = "john.smith";
			var mockCacheProvider = new Mock<ICacheProvider>();

			var mockTeamsService = new Mock<ITeamsService>();
			mockTeamsService.Setup(x => x.GetTeam(expectedTeamOwner)).ReturnsAsync(new ConcernsCaseworkTeamDto(expectedTeamOwner, Array.Empty<string>()));

			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), mockTeamsService.Object, mockCacheProvider.Object);

			// act
			var team = await sut.GetTeam(expectedTeamOwner);

			// assert
			Assert.IsNotNull(team);
		}

		[Test]
		public async Task GetTeam_When_Cached_Returns_CachedData()
		{
			// arrange
			var expectedTeamOwner = "john.smith";
			var expectedCacheKey = "Concerns.Teams.john.smith";
			var mockCacheProvider = new Mock<ICacheProvider>();
			mockCacheProvider.Setup(x => x.GetFromCache<ConcernsCaseworkTeamDto>(expectedCacheKey))
				.ReturnsAsync(new ConcernsCaseworkTeamDto(expectedCacheKey, Array.Empty<string>()));

			var mockTeamsService = new Mock<ITeamsService>();

			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), mockTeamsService.Object, mockCacheProvider.Object);

			// act
			var team = await sut.GetTeam(expectedTeamOwner);

			// assert
			Assert.IsNotNull(team);
		}

		[Test]
		public async Task GetTeamOwners_ByPasses_Cache()
		{
			// arrange
			var expectedTeamOwner = "john.smith";
			var mockCacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);

			var mockTeamsService = new Mock<ITeamsService>();
			mockTeamsService.Setup(x => x.GetTeamOwners()).ReturnsAsync(new[] { expectedTeamOwner });

			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), mockTeamsService.Object, mockCacheProvider.Object);

			// act
			var teamOwners = await sut.GetTeamOwners();

			// assert
			Assert.That(teamOwners.Length == 1);
			Assert.That(teamOwners[0] == expectedTeamOwner);
		}

		[Test]
		public async Task PutTeam_When_Calls_Service_And_Updates_Cache()
		{
			// arrange
			var expectedTeamOwner = "john.smith";
			var expectedCacheKey = "Concerns.Teams.john.smith";

			var updatedTeam = new ConcernsCaseworkTeamDto(expectedTeamOwner, new[] { "user1", "user2" });

			var mockCacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);
			mockCacheProvider.Setup(x => x.ClearCache(expectedCacheKey)).Returns(Task.CompletedTask);
			mockCacheProvider.Setup(x => x.SetCache(expectedCacheKey, updatedTeam, It.IsAny<DistributedCacheEntryOptions>())).Returns(Task.CompletedTask);

			var mockTeamsService = new Mock<ITeamsService>();
			mockTeamsService.Setup(x => x.PutTeam(updatedTeam));

			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), mockTeamsService.Object, mockCacheProvider.Object);

			// act
			await sut.PutTeam(updatedTeam);

			// assert
			mockCacheProvider.Verify(x => x.ClearCache(expectedCacheKey), Times.Once);
			mockTeamsService.Verify(x => x.PutTeam(updatedTeam), Times.Once);
			mockCacheProvider.Verify(x => x.SetCache(expectedCacheKey, updatedTeam, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}
	}
}
