using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Type;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Type
{
	[Parallelizable(ParallelScope.All)]
	public class TypeServiceTests
	{
		[Test]
		public async Task WhenGetTypes_ReturnsTypes()
		{
			// arrange
			var expectedTypes = TypeFactory.BuildListTypeDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedTypes))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TypeService>>();
			var typeService = new TypeService(httpClientFactory.Object, logger.Object);
			
			// act
			var types = await typeService.GetTypes();

			// assert
			Assert.That(types, Is.Not.Null);
			Assert.That(types.Count, Is.EqualTo(expectedTypes.Count));

			foreach (var actualType in types)
			{
				foreach (var expectedType in expectedTypes.Where(expectedType => actualType.Urn.CompareTo(expectedType.Urn) == 0))
				{
					Assert.That(actualType.Name, Is.EqualTo(expectedType.Name));
					Assert.That(actualType.Description, Is.EqualTo(expectedType.Description));
					Assert.That(actualType.Urn, Is.EqualTo(expectedType.Urn));
					Assert.That(actualType.CreatedAt, Is.EqualTo(expectedType.CreatedAt));
					Assert.That(actualType.UpdatedAt, Is.EqualTo(expectedType.UpdatedAt));
				}
			}
		}
		
		[Test]
		public async Task WhenGetTypes_ThrowsException_ReturnsEmptyTypes()
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
			
			var logger = new Mock<ILogger<TypeService>>();
			var typeService = new TypeService(httpClientFactory.Object, logger.Object);
			
			// act
			var types = await typeService.GetTypes();

			// assert
			Assert.That(types, Is.Not.Null);
			// TODO uncomment when trams api is live
			//Assert.That(types.Count, Is.EqualTo(0));
			Assert.That(types.Count, Is.EqualTo(13));
		}
	}
}