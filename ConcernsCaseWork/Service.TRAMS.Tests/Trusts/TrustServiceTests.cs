using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Models;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustServiceTests
	{
		[Test]
		public async Task WhenGetTrustsByPagination_ReturnsTrusts()
		{
			// arrange
			var expectedTrusts = TrustDtoFactory.CreateListTrustDto();
			var configuration = ConfigurationFactory.ConfigurationUserSecretsBuilder();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedTrusts))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act
			var trusts = await trustService.GetTrustsByPagination();

			// assert
			IEnumerable<TrustDto> trustDtos = trusts.ToList();
			Assert.That(trustDtos, Is.Not.Null);
			Assert.That(trustDtos.Count(), Is.EqualTo(expectedTrusts.Count));

		}
	}
}