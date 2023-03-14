namespace ConcernsCaseWork.Service.AzureAd.Tests;
using ConcernsCaseWork.Service.AzureAd.Client;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

public class GraphGroupIdSettingsTests
{
	[Fact]
	public void Can_Populate_Settings_From_IConfiguation()
	{
		var expectedCaseWorkerGroupId = Guid.NewGuid().ToString();
		var expectedTeamLeaderGroupId = Guid.NewGuid().ToString();
		var expectedAdminGroupId = Guid.NewGuid().ToString();

		var fakeSettings = new Dictionary<string, string> {
			{ "AzureAdGroups:CaseWorkerGroupId", expectedCaseWorkerGroupId},
			{ "AzureAdGroups:TeamleaderGroupId", expectedTeamLeaderGroupId},
			{ "AzureAdGroups:AdminGroupId", expectedAdminGroupId},
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(fakeSettings)
			.Build();

		var sut = new GraphGroupIdSettings(configuration);

		sut.CaseWorkerGroupId.Should().Be(expectedCaseWorkerGroupId);
		sut.TeamLeaderGroupId.Should().Be(expectedTeamLeaderGroupId);
		sut.AdminGroupId.Should().Be(expectedAdminGroupId);
	}

	[Fact]
	public void GraphClientSettings_Implements_IGraphClientSettings()
	{
		typeof(GraphClientSettings).Should().Implement<IGraphClientSettings>();
	}
}