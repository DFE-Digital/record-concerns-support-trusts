using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Status;
using Service.TRAMS.Type;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Status
{
	[Parallelizable(ParallelScope.All)]
	public class StatusServiceTests
	{
		[Test]
		public async Task WhenGetStatus_ReturnsStatus()
		{
			// arrange
			var expectedStatus = StatusDtoFactory.BuildListStatusDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedStatus))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<StatusService>>();
			var statusService = new StatusService(httpClientFactory.Object, logger.Object);
			
			// act
			var statuses = await statusService.GetStatuses();

			// assert
			Assert.That(statuses, Is.Not.Null);
			Assert.That(statuses.Count, Is.EqualTo(expectedStatus.Count));

			foreach (var actualStatus in statuses)
			{
				foreach (var expectedType in expectedStatus.Where(expectedType => actualStatus.Urn.CompareTo(expectedType.Urn) == 0))
				{
					Assert.That(actualStatus.Name, Is.EqualTo(expectedType.Name));
					Assert.That(actualStatus.Urn, Is.EqualTo(expectedType.Urn));
					Assert.That(actualStatus.CreatedAt, Is.EqualTo(expectedType.CreatedAt));
					Assert.That(actualStatus.UpdatedAt, Is.EqualTo(expectedType.UpdatedAt));
				}
			}
		}
		
		[Test]
		public async Task WhenGetStatus_ThrowsException_ReturnsEmptyStatuses()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<StatusService>>();
			var statusService = new StatusService(httpClientFactory.Object, logger.Object);
			
			// act
			var statuses = await statusService.GetStatuses();

			// assert
			Assert.That(statuses, Is.Not.Null);
			// TODO uncomment when trams api is live
			//Assert.That(statuses.Count, Is.EqualTo(0));
			Assert.That(statuses.Count, Is.EqualTo(3));
		}
	}
}