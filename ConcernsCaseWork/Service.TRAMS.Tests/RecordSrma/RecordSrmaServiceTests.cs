using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.RecordSrma;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.RecordSrma
{
	[Parallelizable(ParallelScope.All)]
	public class RecordSrmaServiceTests
	{
		[Test]
		public async Task WhenGetRecordsSrmaByRecordUrn_ReturnsRecordsSrmaDto()
		{
			// arrange
			var expectedRecordsSrma = RecordSrmaFactory.BuildListRecordSrmaDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordsSrma))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsSrma = await recordSrmaService.GetRecordsSrmaByRecordUrn(1);

			// assert
			Assert.That(actualRecordsSrma, Is.Not.Null);
			Assert.That(actualRecordsSrma.Count, Is.EqualTo(expectedRecordsSrma.Count));
			
			foreach (var actualRecord in actualRecordsSrma)
			{
				foreach (var expectedRecord in expectedRecordsSrma.Where(r => r.Urn.CompareTo(actualRecord.Urn) == 0))
				{
					Assert.That(actualRecord.Name, Is.EqualTo(expectedRecord.Name));
					Assert.That(actualRecord.Urn, Is.EqualTo(expectedRecord.Urn));
					Assert.That(actualRecord.Details, Is.EqualTo(expectedRecord.Details));
					Assert.That(actualRecord.RecordUrn, Is.EqualTo(expectedRecord.RecordUrn));
					Assert.That(actualRecord.Reason, Is.EqualTo(expectedRecord.Reason));
				}
			}
		}

		[Test]
		public async Task WhenGetRecordsSrmaByRecordUrn_ThrowsException_ReturnsEmptyRecordSrmaDto()
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
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsWhistleblower = await recordSrmaService.GetRecordsSrmaByRecordUrn(1);

			// assert
			Assert.That(actualRecordsWhistleblower, Is.Not.Null);
			Assert.That(actualRecordsWhistleblower.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostRecordSrmaByRecordUrn_ReturnsRecordSrmaDto()
		{
			// arrange
			var expectedRecordSrma = RecordSrmaFactory.BuildCreateRecordSrmaDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordSrma))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordSrma = await recordSrmaService.PostRecordSrmaByRecordUrn(expectedRecordSrma);

			// assert
			Assert.That(actualRecordSrma.Name, Is.EqualTo(expectedRecordSrma.Name));
			Assert.That(actualRecordSrma.Urn, Is.Not.Null);
			Assert.That(actualRecordSrma.Details, Is.EqualTo(expectedRecordSrma.Details));
			Assert.That(actualRecordSrma.RecordUrn, Is.EqualTo(expectedRecordSrma.RecordUrn));
			Assert.That(actualRecordSrma.Reason, Is.EqualTo(expectedRecordSrma.Reason));
		}

		[Test]
		public async Task WhenPostRecordSrmaByRecordUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordSrma = await recordSrmaService.PostRecordSrmaByRecordUrn(RecordSrmaFactory.BuildCreateRecordSrmaDto());

			// assert
			Assert.That(actualRecordSrma, Is.Null);
		}
		
		[Test]
		public async Task WhenPatchRecordSrmaByUrn_ReturnsRecordSrmaDto()
		{
			// arrange
			var expectedRecordSrma = RecordSrmaFactory.BuildRecordSrmaDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordSrma))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordSrma = await recordSrmaService.PatchRecordSrmaByUrn(expectedRecordSrma);

			// assert
			Assert.That(actualRecordSrma.Name, Is.EqualTo(expectedRecordSrma.Name));
			Assert.That(actualRecordSrma.Urn, Is.EqualTo(expectedRecordSrma.Urn));
			Assert.That(actualRecordSrma.Details, Is.EqualTo(expectedRecordSrma.Details));
			Assert.That(actualRecordSrma.RecordUrn, Is.EqualTo(expectedRecordSrma.RecordUrn));
			Assert.That(actualRecordSrma.Reason, Is.EqualTo(expectedRecordSrma.Reason));
		}

		[Test]
		public async Task WhenPatchRecordSrmaByUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordSrmaService>>();
			var recordSrmaService = new RecordSrmaService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordSrma = await recordSrmaService.PatchRecordSrmaByUrn(RecordSrmaFactory.BuildRecordSrmaDto());

			// assert
			Assert.That(actualRecordSrma, Is.Null);
		}
	}
}