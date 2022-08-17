using ConcernsCasework.Service.RecordRatingHistory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.RecordRatingHistory
{
	[Parallelizable(ParallelScope.All)]
	public class RecordRatingHistoryServiceTests
	{
		[Test]
		public async Task WhenGetRecordsRatingHistoryByCaseUrn_ReturnsRecordsRatingHistoryDto()
		{
			// arrange
			var expectedRecordsRatingHistory = RecordRatingHistoryFactory.BuildListRecordRatingHistoryDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordsRatingHistory))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.GetRecordsRatingHistoryByCaseUrn(1);

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Not.Null);
			Assert.That(actualRecordsRatingHistory.Count, Is.EqualTo(expectedRecordsRatingHistory.Count));
			
			foreach (var actualRecord in actualRecordsRatingHistory)
			{
				foreach (var expectedRecord in expectedRecordsRatingHistory.Where(r => r.RecordUrn.CompareTo(actualRecord.RecordUrn) == 0))
				{
					Assert.That(actualRecord.CreatedAt, Is.EqualTo(expectedRecord.CreatedAt));
					Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
					Assert.That(actualRecord.RecordUrn, Is.EqualTo(expectedRecord.RecordUrn));
				}
			}
		}
		
		[Test]
		public async Task WhenGetRecordsRatingHistoryByCaseUrn_ReturnsEmptyRecordsRatingHistoryDto()
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
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.GetRecordsRatingHistoryByCaseUrn(1);

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Not.Null);
			Assert.That(actualRecordsRatingHistory.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetRecordsRatingHistoryByRecordUrn_ReturnsRecordsRatingHistoryDto()
		{
			// arrange
			var expectedRecordsRatingHistory = RecordRatingHistoryFactory.BuildListRecordRatingHistoryDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordsRatingHistory))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.GetRecordsRatingHistoryByRecordUrn(1);

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Not.Null);
			Assert.That(actualRecordsRatingHistory.Count, Is.EqualTo(expectedRecordsRatingHistory.Count));
			
			foreach (var actualRecord in actualRecordsRatingHistory)
			{
				foreach (var expectedRecord in expectedRecordsRatingHistory.Where(r => r.RecordUrn.CompareTo(actualRecord.RecordUrn) == 0))
				{
					Assert.That(actualRecord.CreatedAt, Is.EqualTo(expectedRecord.CreatedAt));
					Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
					Assert.That(actualRecord.RecordUrn, Is.EqualTo(expectedRecord.RecordUrn));
				}
			}
		}
		
		[Test]
		public async Task WhenGetRecordsRatingHistoryByRecordUrn_ReturnsEmptyRecordsRatingHistoryDto()
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
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.GetRecordsRatingHistoryByRecordUrn(1);

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Not.Null);
			Assert.That(actualRecordsRatingHistory.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostRecordRatingHistory_ReturnsRecordRatingHistoryDto()
		{
			// arrange
			var expectedRecordRatingHistory = RecordRatingHistoryFactory.BuildRecordRatingHistoryDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordRatingHistory))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.PostRecordRatingHistory(expectedRecordRatingHistory);

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Not.Null);
			Assert.That(actualRecordsRatingHistory.CreatedAt, Is.EqualTo(expectedRecordRatingHistory.CreatedAt));
			Assert.That(actualRecordsRatingHistory.RatingUrn, Is.EqualTo(expectedRecordRatingHistory.RatingUrn));
			Assert.That(actualRecordsRatingHistory.RecordUrn, Is.EqualTo(expectedRecordRatingHistory.RecordUrn));
		}
		
		[Test]
		public async Task WhenPostRecordRatingHistory_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordRatingHistoryService>>();
			var recordRatingHistoryService = new RecordRatingHistoryService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsRatingHistory = await recordRatingHistoryService.PostRecordRatingHistory(RecordRatingHistoryFactory.BuildRecordRatingHistoryDto());

			// assert
			Assert.That(actualRecordsRatingHistory, Is.Null);
		}
	}
}