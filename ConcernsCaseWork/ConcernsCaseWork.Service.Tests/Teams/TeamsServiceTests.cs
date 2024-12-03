using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Teams;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Teams
{
	public class TeamsServiceTests
	{
		[Test]
		public void Methods_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsService).GetMethods());
		}

		[Test]
		public void Constructors_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsService).GetConstructors());
		}

		[Test]
		public async Task GetTeam_Returns_Dto()
		{
			var expectedDto = new ConcernsCaseworkTeamDto("user.one", new[] { "user.two" });
			var responseWrapper = new ApiWrapper<ConcernsCaseworkTeamDto>(expectedDto);
			var concernsApiEndpoint = "https://localhost";
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(responseWrapper))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());
			var result = await sut.GetTeam("user.one");

			Assert.That(expectedDto.OwnerId, Is.EqualTo(result.OwnerId));
			Assert.That(expectedDto.TeamMembers, Is.EqualTo(result.TeamMembers));
		}

		[Test]
		public async Task GetTeam_Allows_NoContent_Result()
		{
			var concernsApiEndpoint = "https://localhost";
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.NoContent
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());
			var result = await sut.GetTeam("user.one");

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetTeamOwners_Returns_Data()
		{
			var expectedData = new string[] { "user.one", "user.two" };
			var responseWrapper = new ApiWrapper<string[]>(expectedData);
			var concernsApiEndpoint = "https://localhost";
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(responseWrapper))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());
			var result = await sut.GetTeamOwners();

			Assert.That(result, Is.Not.Null);
			Assert.That(2, Is.EqualTo(result.Length));
			Assert.That(result, Does.Contain(expectedData[0]));
			Assert.That(result, Does.Contain(expectedData[1]));
		}

		[Test]
		public async Task GetTeamOwners_Allows_NoContent_Result()
		{
			var concernsApiEndpoint = "https://localhost";
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.NoContent
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());
			var result = await sut.GetTeamOwners();

			Assert.That(result, Is.Not.Null);
			Assert.That(0, Is.EqualTo(result.Length));
		}

		// Todo. Work out how to validate the Put method is working.
	}
}
