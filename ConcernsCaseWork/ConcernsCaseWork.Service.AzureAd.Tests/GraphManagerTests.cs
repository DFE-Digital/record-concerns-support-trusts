using AutoFixture;
using FluentAssertions;
using Moq;

namespace ConcernsCaseWork.Service.AzureAd.Tests;

public class GraphManagerTests
{
	[Fact]
	public void GraphManager_Implements_IGraphManager()
	{
		typeof(GraphManager).Should().Implement<IGraphManager>();
	}

	[Fact]
	public async Task When_GetTeamLeaders_Then_Uses_Correct_GroupId_With_Client()
	{
		ConcernsCaseWorkAdUser[] fakeResults = new Fixture().CreateMany<ConcernsCaseWorkAdUser>(5).ToArray();

		IGraphClientSettings? mockConfig = Mock.Of<IGraphClientSettings>(x => x.TeamLeaderGroupId == Guid.NewGuid().ToString());
		IGraphClient? mockClient =
			Mock.Of<IGraphClient>(x => x.GetCaseWorkersByGroupId(mockConfig.TeamLeaderGroupId, It.IsAny<CancellationToken>()) == Task.FromResult(fakeResults));

		GraphManager sut = new(mockClient, mockConfig);
		ConcernsCaseWorkAdUser[] results = await sut.GetTeamLeaders(CancellationToken.None);

		results.Should().BeEquivalentTo(fakeResults);
		results.All(x => x.IsTeamLeader).Should().BeTrue();
		Mock.Get(mockClient).Verify(x => x.GetCaseWorkersByGroupId(mockConfig.TeamLeaderGroupId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task When_GetCaseWorkers_Then_Uses_Correct_GroupId_With_Client()
	{
		ConcernsCaseWorkAdUser[] fakeResults = new Fixture().CreateMany<ConcernsCaseWorkAdUser>(5).ToArray();
		IGraphClientSettings? mockConfig = Mock.Of<IGraphClientSettings>(x => x.CaseWorkerGroupId == Guid.NewGuid().ToString());
		IGraphClient? mockClient =
			Mock.Of<IGraphClient>(x => x.GetCaseWorkersByGroupId(mockConfig.CaseWorkerGroupId, It.IsAny<CancellationToken>()) == Task.FromResult(fakeResults));

		GraphManager sut = new(mockClient, mockConfig);
		ConcernsCaseWorkAdUser[] results = await sut.GetCaseWorkers(CancellationToken.None);

		results.Should().BeEquivalentTo(fakeResults);
		results.All(x => x.IsCaseworker).Should().BeTrue();
		Mock.Get(mockClient).Verify(x => x.GetCaseWorkersByGroupId(mockConfig.CaseWorkerGroupId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task When_GetAdministrators_Then_Uses_Correct_GroupId_With_Client()
	{
		ConcernsCaseWorkAdUser[] fakeResults = new Fixture().CreateMany<ConcernsCaseWorkAdUser>(5).ToArray();
		IGraphClientSettings? mockConfig = Mock.Of<IGraphClientSettings>(x => x.AdminGroupId == Guid.NewGuid().ToString());
		IGraphClient? mockClient = Mock.Of<IGraphClient>(x => x.GetCaseWorkersByGroupId(mockConfig.AdminGroupId, It.IsAny<CancellationToken>()) == Task.FromResult(fakeResults));

		GraphManager sut = new(mockClient, mockConfig);
		ConcernsCaseWorkAdUser[] results = await sut.GetAdministrators(CancellationToken.None);

		results.Should().BeEquivalentTo(fakeResults);
		results.All(x => x.IsAdmin).Should().BeTrue();
		Mock.Get(mockClient).Verify(x => x.GetCaseWorkersByGroupId(mockConfig.AdminGroupId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task When_GetAllUsers_Then_Returns_All_UserTypes()
	{
		Fixture fixture = new();
		var fakeCaseworkers = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, true).With(x => x.IsTeamLeader, false).With(x => x.IsAdmin, false).CreateMany(20).ToArray();
		var fakeTeamLeaders = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, false).With(x => x.IsTeamLeader, true).With(x => x.IsAdmin, false).CreateMany(10).ToArray();
		var fakeAdmins = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, false).With(x => x.IsTeamLeader, false).With(x => x.IsAdmin, true).CreateMany(5).ToArray();

		List<ConcernsCaseWorkAdUser> allFakeResults = new();
		allFakeResults.AddRange(fakeCaseworkers);
		allFakeResults.AddRange(fakeTeamLeaders);
		allFakeResults.AddRange(fakeAdmins);

		IGraphClientSettings? mockConfig = Mock.Of<IGraphClientSettings>(
			x => x.CaseWorkerGroupId == Guid.NewGuid().ToString() &&
			     x.TeamLeaderGroupId == Guid.NewGuid().ToString() &&
			     x.AdminGroupId == Guid.NewGuid().ToString()
		);

		Mock<IGraphClient> mockClient = new();
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.CaseWorkerGroupId, It.IsAny<CancellationToken>())).ReturnsAsync(fakeCaseworkers);
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.TeamLeaderGroupId, It.IsAny<CancellationToken>())).ReturnsAsync(fakeTeamLeaders);
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.AdminGroupId, It.IsAny<CancellationToken>())).ReturnsAsync(fakeAdmins);

		GraphManager sut = new(mockClient.Object, mockConfig);
		ConcernsCaseWorkAdUser[] results = await sut.GetAllUsers(CancellationToken.None);

		results.Should().BeEquivalentTo(allFakeResults);
	}

	[Fact]
	public async Task When_GetAllUsers_Finds_Users_In_Multiple_Groups_Then_Users_Are_Aggregated()
	{
		const int usersInEachGroup = 10;
		Fixture fixture = new();
		var fakeUsersInAllGroups = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, true).With(x => x.IsTeamLeader, true).With(x => x.IsAdmin, true).CreateMany(usersInEachGroup).ToArray();
		var fakeCaseworkers = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, true).With(x => x.IsTeamLeader, false).With(x => x.IsAdmin, false).CreateMany(usersInEachGroup).ToArray();
		var fakeTeamLeaders = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, false).With(x => x.IsTeamLeader, true).With(x => x.IsAdmin, false).CreateMany(usersInEachGroup).ToArray();
		var fakeAdmins = fixture.Build<ConcernsCaseWorkAdUser>().With(x => x.IsCaseworker, false).With(x => x.IsTeamLeader, false).With(x => x.IsAdmin, true).CreateMany(usersInEachGroup).ToArray();
		
		IGraphClientSettings? mockConfig = Mock.Of<IGraphClientSettings>(
			x => x.CaseWorkerGroupId == Guid.NewGuid().ToString() &&
			     x.TeamLeaderGroupId == Guid.NewGuid().ToString() &&
			     x.AdminGroupId == Guid.NewGuid().ToString()
		);

		Mock<IGraphClient> mockClient = new();
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.CaseWorkerGroupId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(fakeCaseworkers.Concat(fakeUsersInAllGroups).ToArray());
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.TeamLeaderGroupId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(fakeTeamLeaders.Concat(fakeUsersInAllGroups).ToArray());
		mockClient.Setup(x => x.GetCaseWorkersByGroupId(mockConfig.AdminGroupId, It.IsAny<CancellationToken>())).ReturnsAsync(fakeAdmins.Concat(fakeUsersInAllGroups).ToArray());

		GraphManager sut = new(mockClient.Object, mockConfig);
		ConcernsCaseWorkAdUser[] results = await sut.GetAllUsers(CancellationToken.None);

		results.Length.Should().Be(usersInEachGroup * 4);
		results.Where(x => x is { IsCaseworker: true, IsTeamLeader: true, IsAdmin: true }).Should().BeEquivalentTo(fakeUsersInAllGroups);
		results.Where(x => x.IsCaseworker).Should().BeEquivalentTo(fakeCaseworkers.Concat(fakeUsersInAllGroups));
		results.Where(x => x.IsTeamLeader).Should().BeEquivalentTo(fakeTeamLeaders.Concat(fakeUsersInAllGroups));
		results.Where(x => x.IsAdmin).Should().BeEquivalentTo(fakeAdmins.Concat(fakeUsersInAllGroups));
	}
}