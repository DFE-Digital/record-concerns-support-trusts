using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.RecordAcademy;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.RecordAcademy
{
	[Parallelizable(ParallelScope.All)]
	public class RecordAcademyServiceTests
	{
		[Test]
		public async Task WhenGetRecordsAcademyByRecordUrn_ReturnsRecordsAcademyDto()
		{
			// arrange
			var expectedRecordsAcademy = RecordAcademyFactory.BuildListRecordAcademyDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordsAcademy))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordsAcademy = await recordSrmaService.GetRecordsAcademyByRecordUrn(1);

			// assert
			Assert.That(actualRecordsAcademy, Is.Not.Null);
			Assert.That(actualRecordsAcademy.Count, Is.EqualTo(expectedRecordsAcademy.Count));
			
			foreach (var actualRecordAcademy in actualRecordsAcademy)
			{
				foreach (var expectedRecordAcademy in expectedRecordsAcademy.Where(r => r.Urn.CompareTo(actualRecordAcademy.Urn) == 0))
				{
					Assert.That(actualRecordAcademy.AcademyUrn, Is.EqualTo(expectedRecordAcademy.AcademyUrn));
					Assert.That(actualRecordAcademy.Urn, Is.EqualTo(expectedRecordAcademy.Urn));
					Assert.That(actualRecordAcademy.RecordUrn, Is.EqualTo(expectedRecordAcademy.RecordUrn));
				}
			}
		}

		[Test]
		public async Task WhenGetRecordsAcademyByRecordUrn_ThrowsException_ReturnsEmptyRecordAcademyDto()
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
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordsAcademy = await recordSrmaService.GetRecordsAcademyByRecordUrn(1);

			// assert
			Assert.That(actualRecordsAcademy, Is.Not.Null);
			Assert.That(actualRecordsAcademy.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostRecordAcademyByRecordUrn_ReturnsRecordAcademyDto()
		{
			// arrange
			var expectedRecordAcademy = RecordAcademyFactory.BuildRecordAcademyDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(RecordSrmaFactory.BuildRecordSrmaDto()))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordAcademy = await recordSrmaService.PostRecordAcademyByRecordUrn(RecordAcademyFactory.BuildCreateRecordAcademyDto());

			// assert
			Assert.That(actualRecordAcademy.AcademyUrn, Is.Not.Null);
			Assert.That(actualRecordAcademy.Urn, Is.EqualTo(expectedRecordAcademy.Urn));
			Assert.That(actualRecordAcademy.RecordUrn, Is.EqualTo(expectedRecordAcademy.RecordUrn));
		}

		[Test]
		public async Task WhenPostRecordAcademyByRecordUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordAcademy = await recordSrmaService.PostRecordAcademyByRecordUrn(RecordAcademyFactory.BuildCreateRecordAcademyDto());

			// assert
			Assert.That(actualRecordAcademy, Is.Null);
		}
		
		[Test]
		public async Task WhenPatchRecordAcademyByUrn_ReturnsRecordAcademyDto()
		{
			// arrange
			var expectedRecordAcademy = RecordAcademyFactory.BuildRecordAcademyDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRecordAcademy))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordAcademy = await recordSrmaService.PatchRecordAcademyByUrn(expectedRecordAcademy);

			// assert
			Assert.That(actualRecordAcademy.AcademyUrn, Is.EqualTo(expectedRecordAcademy.AcademyUrn));
			Assert.That(actualRecordAcademy.Urn, Is.EqualTo(expectedRecordAcademy.Urn));
			Assert.That(actualRecordAcademy.RecordUrn, Is.EqualTo(expectedRecordAcademy.RecordUrn));
		}

		[Test]
		public async Task WhenPatchRecordAcademyByUrn_ThrowsException_ReturnsNull()
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
			
			var logger = new Mock<ILogger<RecordAcademyService>>();
			var recordSrmaService = new RecordAcademyService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var actualRecordAcademy = await recordSrmaService.PatchRecordAcademyByUrn(RecordAcademyFactory.BuildRecordAcademyDto());

			// assert
			Assert.That(actualRecordAcademy, Is.Null);
		}
	}
}