using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
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
			var expectedRecords = RecordFactory.BuildListRecordDto();
			var apiListWrapperRecords = new ApiListWrapper<RecordDto>(expectedRecords, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperRecords))
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
					Assert.That(actualRecord.StatusUrn, Is.EqualTo(expectedRecord.StatusUrn));
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
		public void WhenGetRecordsByCaseUrn_ThrowsException()
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
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => recordService.GetRecordsByCaseUrn(1));
		}
		
		[Test]
		public void WhenGetRecordsByCaseUrn_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiListWrapperRecords = new ApiListWrapper<RecordDto>(null, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperRecords))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => recordService.GetRecordsByCaseUrn(1));
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnsRecord()
		{
			// arrange
			var expectedRecord = RecordFactory.BuildCreateRecordDto();
			var apiWrapperRecord = new ApiWrapper<CreateRecordDto>(expectedRecord);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiWrapperRecord))
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
			Assert.That(actualRecord.CreatedAt, Is.EqualTo(expectedRecord.CreatedAt));
			Assert.That(actualRecord.Description, Is.EqualTo(expectedRecord.Description));
			Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
			Assert.That(actualRecord.StatusUrn, Is.EqualTo(expectedRecord.StatusUrn));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
		}

		[Test]
		public void WhenPostRecordByCaseUrn_ThrowsException()
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
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => recordService.PostRecordByCaseUrn(RecordFactory.BuildCreateRecordDto()));
		}
		
		[Test]
		public void WhenPostRecordByCaseUrn_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiWrapperRecord = new ApiWrapper<RecordDto>(null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiWrapperRecord))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => recordService.PostRecordByCaseUrn(RecordFactory.BuildCreateRecordDto()));
		}
		
		[Test]
		public async Task WhenPatchRecordByUrn_ReturnsRecord()
		{
			// arrange
			var expectedRecord = RecordFactory.BuildRecordDto();
			var apiWrapperRecord = new ApiWrapper<RecordDto>(expectedRecord);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiWrapperRecord))
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
			Assert.That(actualRecord.StatusUrn, Is.EqualTo(expectedRecord.StatusUrn));
			Assert.That(actualRecord.CaseUrn, Is.EqualTo(expectedRecord.CaseUrn));
			Assert.That(actualRecord.ClosedAt, Is.EqualTo(expectedRecord.ClosedAt));
			Assert.That(actualRecord.RatingUrn, Is.EqualTo(expectedRecord.RatingUrn));
			Assert.That(actualRecord.ReviewAt, Is.EqualTo(expectedRecord.ReviewAt));
			Assert.That(actualRecord.TypeUrn, Is.EqualTo(expectedRecord.TypeUrn));
			Assert.That(actualRecord.UpdatedAt, Is.EqualTo(expectedRecord.UpdatedAt));
		}

		[Test]
		public void WhenPatchRecordByUrn_ThrowsException()
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
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => recordService.PatchRecordByUrn(RecordFactory.BuildRecordDto()));
		}
		
		[Test]
		public void WhenPatchRecordByUrn_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiWrapperRecord = new ApiWrapper<RecordDto>(null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiWrapperRecord))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordService>>();
			var recordService = new RecordService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => recordService.PatchRecordByUrn(RecordFactory.BuildRecordDto()));
		}
	}
}