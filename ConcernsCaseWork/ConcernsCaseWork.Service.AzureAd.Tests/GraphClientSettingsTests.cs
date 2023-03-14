using ConcernsCaseWork.Service.AzureAd.Client;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Service.AzureAd.Tests
{
	public class GraphClientSettingsTests
	{
		[Fact]
		public void Can_Populate_Settings_From_IConfiguation()
		{
			var expectedGraphEndpointScope = $"random-string: {Guid.NewGuid()}";

			var expectedTenantId = Guid.NewGuid().ToString();
			var expectedClientId = Guid.NewGuid().ToString();
			var expectedClientSecret = Guid.NewGuid().ToString();

			var fakeSettings = new Dictionary<string, string> {
			  { "AzureAdGroups:GraphEndpointScope", expectedGraphEndpointScope},
			  { "AzureAd:TenantId", expectedTenantId },
			  { "AzureAd:ClientId", expectedClientId },
			  { "AzureAd:ClientSecret", expectedClientSecret },
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(fakeSettings)
				.Build();

			var sut = new GraphClientSettings(configuration);

			sut.TenantId.Should().Be(expectedTenantId);
			sut.ClientId.Should().Be(expectedClientId);
			sut.ClientSecret.Should().Be(expectedClientSecret);
			sut.GraphEndpointScope.Should().Be(expectedGraphEndpointScope);
		}

		[Fact]
		public void GraphClientSettings_Implements_IGraphClientSettings()
		{
			typeof(GraphClientSettings).Should().Implement<IGraphClientSettings>();
		}
	}
}