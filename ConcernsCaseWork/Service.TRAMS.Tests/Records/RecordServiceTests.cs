using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Records;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Records
{
	[Parallelizable(ParallelScope.All)]
	public class RecordServiceTests
	{
		[Test]
		public async Task WhenGetRecordsByCaseUrn_ReturnsRecords()
		{
			// arrange
			var expectedRecords = RecordDtoFactory.BuildListRecordDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecords))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecords = await recordService.GetRecordsByCaseUrn(1);

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(expectedRecords.Count));
			
			foreach (var actualRecord in actualRecords)
			{
				foreach (var expectedRecord in expectedRecords.Where(r => r.Urn.CompareTo(actualRecord.Urn) == 0))
				{
					Assert.That(actualRecord.Name, Is.EqualTo(expectedRecord.Name));
					Assert.That(actualRecord.Urn, Is.EqualTo(expectedRecord.Urn));
					Assert.That(actualRecord.CreatedAt, Is.EqualTo(expectedRecord.CreatedAt));
					Assert.That(actualRecord.Description, Is.EqualTo(expectedRecord.Description));
					Assert.That(actualRecord.Primary, Is.EqualTo(expectedRecord.Primary));
					Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
					Assert.That(actualRecord.Status, Is.EqualTo(expectedRecord.Status));
					Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
					Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
					Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
					Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
					Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
					Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
				}
			}
		}

		[Test]
		public async Task WhenGetRecordsByCaseUrn_ThrowsException_ReturnsEmptyRecords()
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
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecords = await recordService.GetRecordsByCaseUrn(1);

			// assert
			Assert.That(actualRecords, Is.Not.Null);
			Assert.That(actualRecords.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecord()
		{
			// arrange
			var expectedRecord = RecordDtoFactory.BuildCreateRecordDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecord))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecord = await recordService.PostRecordByCaseUrn(expectedRecord);

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(expectedRecord.Name));
			Assert.That(actualRecord.Urn, Is.EqualTo(expectedRecord.Urn));
			Assert.That(actualRecord.CreatedAt, Is.EqualTo(expectedRecord.CreatedAt));
			Assert.That(actualRecord.Description, Is.EqualTo(expectedRecord.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(expectedRecord.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
			Assert.That(actualRecord.Status, Is.EqualTo(expectedRecord.Status));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
		}

		[Test]
		public async Task WhenPostRecordByCaseUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecord = await recordService.PostRecordByCaseUrn(RecordDtoFactory.BuildCreateRecordDto());

			// assert
			Assert.That(actualRecord, Is.Null);
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsRecord()
		{
			// arrange
			var expectedRecord = RecordDtoFactory.BuildUpdateRecordDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecord))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecord = await recordService.PatchRecordByUrn(expectedRecord);

			// assert
			Assert.That(actualRecord, Is.Not.Null);
			Assert.That(actualRecord.Name, Is.EqualTo(expectedRecord.Name));
			Assert.That(actualRecord.Urn, Is.EqualTo(expectedRecord.Urn));
			Assert.That(actualRecord.CreatedAt, Is.Not.Null);
			Assert.That(actualRecord.Description, Is.EqualTo(expectedRecord.Description));
			Assert.That(actualRecord.Primary, Is.EqualTo(expectedRecord.Primary));
			Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
			Assert.That(actualRecord.Status, Is.EqualTo(expectedRecord.Status));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
		}

		[Test]
		public async Task WhenPatchRecordByUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecord = await recordService.PatchRecordByUrn(RecordDtoFactory.BuildUpdateRecordDto());

			// assert
			Assert.That(actualRecord, Is.Null);
		}
	}
}