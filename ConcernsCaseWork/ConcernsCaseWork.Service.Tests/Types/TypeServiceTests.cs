using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Types
{
	[Parallelizable(ParallelScope.All)]
	public class TypeServiceTests
	{
		[Test]
		public async Task WhenGetTypes_ReturnsTypes()
		{
			// arrange
			var expectedTypes = TypeFactory.BuildListTypeDto();
			var apiListWrapperTypes = new ApiListWrapper<TypeDto>(expectedTypes, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperTypes))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TypeService>>();
			var typeService = new TypeService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act
			var types = await typeService.GetTypes();

			// assert
			Assert.That(types, Is.Not.Null);
			Assert.That(types.Count, Is.EqualTo(expectedTypes.Count));

			foreach (var actualType in types)
			{
				foreach (var expectedType in expectedTypes.Where(expectedType => actualType.Id.CompareTo(expectedType.Id) == 0))
				{
					Assert.That(actualType.Name, Is.EqualTo(expectedType.Name));
					Assert.That(actualType.Description, Is.EqualTo(expectedType.Description));
					Assert.That(actualType.Id, Is.EqualTo(expectedType.Id));
					Assert.That(actualType.CreatedAt, Is.EqualTo(expectedType.CreatedAt));
					Assert.That(actualType.UpdatedAt, Is.EqualTo(expectedType.UpdatedAt));
				}
			}
		}
		
		[Test]
		public void WhenGetTypes_ThrowsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TypeService>>();
			var typeService = new TypeService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => typeService.GetTypes());
		}
		
		[Test]
		public void WhenGetTypes_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiListWrapperTypes = new ApiListWrapper<TypeDto>(null, null);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperTypes))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TypeService>>();
			var typeService = new TypeService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>());
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => typeService.GetTypes());
		}
	}
}