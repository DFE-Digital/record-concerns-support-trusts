using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Teams;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Teams
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
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
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
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>());
			var result = await sut.GetTeam("user.one");

			Assert.AreEqual(expectedDto.OwnerId, result.OwnerId);
			Assert.AreEqual(expectedDto.TeamMembers, result.TeamMembers);
		}

		[Test]
		public async Task GetTeam_Allows_NoContent_Result()
		{
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.NoContent
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>());
			var result = await sut.GetTeam("user.one");

			Assert.IsNull(result);
		}

		[Test]
		public async Task GetTeamOwners_Returns_Data()
		{
			var expectedData = new string[] { "user.one", "user.two" };
			var responseWrapper = new ApiWrapper<string[]>(expectedData);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
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
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>());
			var result = await sut.GetTeamOwners();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.Contains(expectedData[0], result);
			Assert.Contains(expectedData[1], result);
		}

		[Test]
		public async Task GetTeamOwners_Allows_NoContent_Result()
		{
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.NoContent
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamsService(httpClientFactory.Object, Mock.Of<ILogger<TeamsService>>(), Mock.Of<ICorrelationContext>());
			var result = await sut.GetTeamOwners();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);
		}

		// Todo. Work out how to validate the Put method is working.
	}
}
