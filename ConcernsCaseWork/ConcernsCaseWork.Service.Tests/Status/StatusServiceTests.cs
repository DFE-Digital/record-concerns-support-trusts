﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Status
{
	[Parallelizable(ParallelScope.All)]
	public class StatusServiceTests
	{
		[Test]
		public async Task WhenGetStatus_ReturnsStatus()
		{
			// arrange
			var expectedStatuses = StatusFactory.BuildListStatusDto();
			var expectedApiWrapperStatuses = new ApiListWrapper<StatusDto>(expectedStatuses, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperStatuses))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<StatusService>>();
			var statusService = new StatusService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var statuses = await statusService.GetStatuses();

			// assert
			Assert.That(statuses, Is.Not.Null);
			Assert.That(statuses.Count, Is.EqualTo(expectedStatuses.Count));

			foreach (var actualStatus in statuses)
			{
				foreach (var expectedStatus in expectedStatuses.Where(s => actualStatus.Id.CompareTo(s.Id) == 0))
				{
					Assert.That(actualStatus.Name, Is.EqualTo(expectedStatus.Name));
					Assert.That(actualStatus.Id, Is.EqualTo(expectedStatus.Id));
					Assert.That(actualStatus.CreatedAt, Is.EqualTo(expectedStatus.CreatedAt));
					Assert.That(actualStatus.UpdatedAt, Is.EqualTo(expectedStatus.UpdatedAt));
				}
			}
		}
		
		[Test]
		public void WhenGetStatus_ApiWrapperResponseDataIsNull_ThrowsException()
		{
			// arrange
			var expectedApiWrapperStatuses = new ApiListWrapper<StatusDto>(null, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperStatuses))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<StatusService>>();
			var statusService = new StatusService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act | assert
			Assert.ThrowsAsync<Exception>(() => statusService.GetStatuses());
		}		
		
		[Test]
		public void WhenGetStatus_ThrowsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<StatusService>>();
			var statusService = new StatusService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act | assert
			Assert.ThrowsAsync<HttpRequestException>(() => statusService.GetStatuses());
		}
	}
}