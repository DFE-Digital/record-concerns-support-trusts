using ConcernsCaseWork.Service.AzureAd.Factories;
using ConcernsCaseWork.Service.AzureAd.Options;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace ConcernsCaseWork.Service.Tests.AzureAd;

public class GraphClientFactoryTests
{
	[Test]
	public void Create_ReturnsGraphClient()
	{
		var options = new AzureAdOptions
		{
			TenantId = Guid.NewGuid(),
			ClientSecret = "client-secret",
			ClientId = Guid.NewGuid()
		};

		var mockOptions = new Mock<IOptions<AzureAdOptions>>();

		mockOptions.SetupGet(m => m.Value).Returns(options);
		GraphClientFactory sut = new(mockOptions.Object);

		Microsoft.Graph.GraphServiceClient client = sut.Create();

		Assert.That(client.BaseUrl, Is.EqualTo($"{options.ApiUrl}/V1.0"));
	}
}
