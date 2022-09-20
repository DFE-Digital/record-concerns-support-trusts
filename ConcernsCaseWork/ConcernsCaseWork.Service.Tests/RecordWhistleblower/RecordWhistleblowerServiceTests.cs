using ConcernsCasework.Service.RecordWhistleblower;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.RecordWhistleblower
{
	[Parallelizable(ParallelScope.All)]
	public class RecordWhistleblowerServiceTests
	{
		[Test]
		public async Task WhenGetRecordsWhistleblowerByRecordUrn_ReturnsRecordsWhistleblowerDto()
		{
			// arrange
			var expectedRecordWhistleblower = RecordWhistleblowerFactory.BuildListRecordWhistleblowerDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordWhistleblower))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsWhistleblower = await recordWhistleblowerService.GetRecordsWhistleblowerByRecordUrn(1);

			// assert
			Assert.That(actualRecordsWhistleblower, Is.Not.Null);
			Assert.That(actualRecordsWhistleblower.Count, Is.EqualTo(expectedRecordWhistleblower.Count));
			
			foreach (var actualRecord in actualRecordsWhistleblower)
			{
				foreach (var expectedRecord in expectedRecordWhistleblower.Where(r => r.Urn.CompareTo(actualRecord.Urn) == 0))
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
		public async Task WhenGetRecordsWhistleblowerByRecordUrn_ThrowsException_ReturnsEmptyRecordWhistleblowerDto()
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
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordsWhistleblower = await recordWhistleblowerService.GetRecordsWhistleblowerByRecordUrn(1);

			// assert
			Assert.That(actualRecordsWhistleblower, Is.Not.Null);
			Assert.That(actualRecordsWhistleblower.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostRecordWhistleblowerByRecordUrn_ReturnsRecordWhistleblowerDto()
		{
			// arrange
			var expectedRecordWhistleblower = RecordWhistleblowerFactory.BuildCreateRecordWhistleblowerDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordWhistleblower))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordWhistleblower = await recordWhistleblowerService.PostRecordWhistleblowerByRecordUrn(expectedRecordWhistleblower);

			// assert
			Assert.That(actualRecordWhistleblower.Name, Is.EqualTo(expectedRecordWhistleblower.Name));
			Assert.That(actualRecordWhistleblower.Details, Is.EqualTo(expectedRecordWhistleblower.Details));
			Assert.That(actualRecordWhistleblower.RecordUrn, Is.EqualTo(expectedRecordWhistleblower.RecordUrn));
			Assert.That(actualRecordWhistleblower.Reason, Is.EqualTo(expectedRecordWhistleblower.Reason));
		}

		[Test]
		public async Task WhenPostRecordWhistleblowerByRecordUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordWhistleblower = await recordWhistleblowerService.PostRecordWhistleblowerByRecordUrn(RecordWhistleblowerFactory.BuildCreateRecordWhistleblowerDto());

			// assert
			Assert.That(actualRecordWhistleblower, Is.Null);
		}
		
		[Test]
		public async Task WhenPatchRecordWhistleblowerByUrn_ReturnsRecordWhistleblowerDto()
		{
			// arrange
			var expectedRecordWhistleblower = RecordWhistleblowerFactory.BuildRecordWhistleblowerDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordWhistleblower))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordWhistleblower = await recordWhistleblowerService.PatchRecordWhistleblowerByUrn(expectedRecordWhistleblower);

			// assert
			Assert.That(actualRecordWhistleblower.Name, Is.EqualTo(expectedRecordWhistleblower.Name));
			Assert.That(actualRecordWhistleblower.Urn, Is.EqualTo(expectedRecordWhistleblower.Urn));
			Assert.That(actualRecordWhistleblower.Details, Is.EqualTo(expectedRecordWhistleblower.Details));
			Assert.That(actualRecordWhistleblower.RecordUrn, Is.EqualTo(expectedRecordWhistleblower.RecordUrn));
			Assert.That(actualRecordWhistleblower.Reason, Is.EqualTo(expectedRecordWhistleblower.Reason));
		}

		[Test]
		public async Task WhenPatchRecordWhistleblowerByUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordWhistleblowerService>>();
			var recordWhistleblowerService = new RecordWhistleblowerService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualRecordWhistleblower = await recordWhistleblowerService.PatchRecordWhistleblowerByUrn(RecordWhistleblowerFactory.BuildRecordWhistleblowerDto());

			// assert
			Assert.That(actualRecordWhistleblower, Is.Null);
		}
	}
}