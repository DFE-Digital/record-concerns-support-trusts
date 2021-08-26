using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
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
	public class CaseServiceTests
	{
		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCases()
		{
			// arrange
			var expectedCases = CaseDtoFactory.CreateListCaseDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCases))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var cases = await caseService.GetCasesByCaseworker("caseworker");

			// assert
			Assert.That(cases, Is.Not.Null);
			Assert.That(cases.Count, Is.EqualTo(expectedCases.Count));
			
			foreach (var caseDto in cases)
			{
				foreach (var expectedCase in expectedCases.Where(expectedCase => caseDto.Id == expectedCase.Id))
				{
					Assert.That(caseDto.Id, Is.EqualTo(expectedCase.Id));
					Assert.That(caseDto.Rag, Is.EqualTo(expectedCase.Rag));
					Assert.That(caseDto.Type, Is.EqualTo(expectedCase.Type));
					Assert.That(caseDto.DaysOpen, Is.EqualTo(expectedCase.DaysOpen));
					Assert.That(caseDto.TrustName, Is.EqualTo(expectedCase.TrustName));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_ThrowsException_ReturnsEmptyCases()
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
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var trusts = await caseService.GetCasesByCaseworker("caseworker");

			// assert
			Assert.That(trusts, Is.Not.Null);
			// Uncomment when pointing service to real trams api
			// Assert.That(trusts.Count, Is.EqualTo(0));
			Assert.That(trusts.Count, Is.EqualTo(5));
		}
	}
}