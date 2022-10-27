using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseHistoryServiceTests
	{
		[Test]
		public async Task WhenPostCaseHistory_ReturnsCaseHistory()
		{
			// arrange
			var expectedCaseHistory = CaseFactory.BuildCaseHistoryDto();
			var expectedCaseHistoryWrap = new ApiWrapper<CaseHistoryDto>(expectedCaseHistory);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseHistoryService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseHistoryWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseHistoryService = new CaseHistoryService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualCaseHistory = await caseHistoryService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto());

			// assert
			Assert.That(actualCaseHistory, Is.Not.Null);
			Assert.That(actualCaseHistory.Description, Is.EqualTo(expectedCaseHistory.Description));
			Assert.That(actualCaseHistory.Action, Is.EqualTo(expectedCaseHistory.Action));
			Assert.That(actualCaseHistory.Title, Is.EqualTo(expectedCaseHistory.Title));
			Assert.That(actualCaseHistory.Urn, Is.EqualTo(expectedCaseHistory.Urn));
			Assert.That(actualCaseHistory.CaseUrn, Is.EqualTo(expectedCaseHistory.CaseUrn));
			Assert.That(actualCaseHistory.CreatedAt, Is.EqualTo(expectedCaseHistory.CreatedAt));
		}
		
		[Test]
		public void WhenPostCaseHistory_ReturnsException()
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
			
			var logger = new Mock<ILogger<CaseHistoryService>>();
			var caseHistoryService = new CaseHistoryService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			Assert.ThrowsAsync<HttpRequestException>(() => caseHistoryService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto()));
		}
		
		[Test]
		public void WhenPostCaseHistory_UnwrapResponse_ReturnsException()
		{
			// arrange
			var expectedCaseHistoryWrap = new ApiWrapper<CaseHistoryDto>(null);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseHistoryWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseHistoryService>>();
			var caseHistoryService = new CaseHistoryService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			Assert.ThrowsAsync<Exception>(() => caseHistoryService.PostCaseHistory(CaseFactory.BuildCreateCaseHistoryDto()));
		}
		
		[Test]
		public async Task WhenGetCasesHistory_ReturnsCasesHistory()
		{
			// arrange
			var expectedCasesHistory = CaseFactory.BuildListCasesHistoryDto();
			var expectedCasesHistoryWrap = new ApiListWrapper<CaseHistoryDto>(expectedCasesHistory, null);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseHistoryService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCasesHistoryWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseHistoryService = new CaseHistoryService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualListWrapperCasesHistory = await caseHistoryService.GetCasesHistory(CaseFactory.BuildCaseSearch());

			// assert
			Assert.That(actualListWrapperCasesHistory, Is.Not.Null);
			Assert.That(actualListWrapperCasesHistory.Data, Is.Not.Null);
			Assert.That(actualListWrapperCasesHistory.Paging, Is.Null);
			Assert.That(actualListWrapperCasesHistory.Data.Count, Is.EqualTo(expectedCasesHistoryWrap.Data.Count));
			
			foreach (var caseHistoryDto in actualListWrapperCasesHistory.Data)
			{
				foreach (var expectedCaseHistory in expectedCasesHistoryWrap.Data.Where(expectedCase => caseHistoryDto.CaseUrn == expectedCase.CaseUrn))
				{
					Assert.That(caseHistoryDto.Description, Is.EqualTo(expectedCaseHistory.Description));
					Assert.That(caseHistoryDto.Action, Is.EqualTo(expectedCaseHistory.Action));
					Assert.That(caseHistoryDto.Title, Is.EqualTo(expectedCaseHistory.Title));
					Assert.That(caseHistoryDto.Urn, Is.EqualTo(expectedCaseHistory.Urn));
					Assert.That(caseHistoryDto.CaseUrn, Is.EqualTo(expectedCaseHistory.CaseUrn));
					Assert.That(caseHistoryDto.CreatedAt, Is.EqualTo(expectedCaseHistory.CreatedAt));
				}
			}
		}
		
		[Test]
		public void WhenGetCasesHistory_ReturnsException()
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
			
			var logger = new Mock<ILogger<CaseHistoryService>>();
			var caseHistoryService = new CaseHistoryService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			Assert.ThrowsAsync<HttpRequestException>(() => caseHistoryService.GetCasesHistory(CaseFactory.BuildCaseSearch()));
		}
	}
}