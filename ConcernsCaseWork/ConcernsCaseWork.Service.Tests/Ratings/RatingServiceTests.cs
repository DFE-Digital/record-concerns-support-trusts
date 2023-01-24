using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Ratings
{
	[Parallelizable(ParallelScope.All)]
	public class RatingServiceTests
	{
		[Test]
		public async Task WhenGetRatings_ReturnsTypes()
		{
			// arrange
			var expectedRatings = new ApiListWrapper<RatingDto>(RatingFactory.BuildListRatingDto(), null);

			var concernsApiEndpoint = "https://localhost";

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedRatings))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var logger = new Mock<ILogger<RatingService>>();
			var ratingService = new RatingService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// act
			var ratings = await ratingService.GetRatings();

			// assert
			Assert.That(ratings, Is.Not.Null);
			Assert.That(ratings.Count, Is.EqualTo(expectedRatings.Data.Count));

			foreach (var actualRating in ratings)
			{
				foreach (var expectedRating in expectedRatings.Data.Where(expectedType => actualRating.Id.CompareTo(expectedType.Id) == 0))
				{
					Assert.That(actualRating.Name, Is.EqualTo(expectedRating.Name));
					Assert.That(actualRating.Id, Is.EqualTo(expectedRating.Id));
					Assert.That(actualRating.CreatedAt, Is.EqualTo(expectedRating.CreatedAt));
					Assert.That(actualRating.UpdatedAt, Is.EqualTo(expectedRating.UpdatedAt));
				}
			}
		}

		[Test]
		public void WhenGetRatings_BadRequest_ThrowsException()
		{
			// arrange
			var concernsApiEndpoint = "https://localhost";

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

			var logger = new Mock<ILogger<RatingService>>();
			var ratingService = new RatingService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			Assert.ThrowsAsync<HttpRequestException>(() => ratingService.GetRatings());
		}
		
		[Test]
		public void WhenGetTypes_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiListWrapperTypes = new ApiListWrapper<RatingDto>(null, null);
			
			var concernsApiEndpoint = "https://localhost";
			
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
			
			var logger = new Mock<ILogger<RatingService>>();
			var ratingService = new RatingService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => ratingService.GetRatings());
		}
	}
}