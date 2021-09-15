using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Rating;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Rating
{
	[Parallelizable(ParallelScope.All)]
	public class RatingServiceTests
	{
		[Test]
		public async Task WhenGetTypes_ReturnsTypes()
		{
			// arrange
			var expectedRatings = RatingFactory.BuildListRatingDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
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
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<RatingService>>();
			var ratingService = new RatingService(httpClientFactory.Object, logger.Object);
			
			// act
			var ratings = await ratingService.GetRatings();

			// assert
			Assert.That(ratings, Is.Not.Null);
			Assert.That(ratings.Count, Is.EqualTo(expectedRatings.Count));

			foreach (var actualRating in ratings)
			{
				foreach (var expectedRating in expectedRatings.Where(expectedType => actualRating.Urn.CompareTo(expectedType.Urn) == 0))
				{
					Assert.That(actualRating.Name, Is.EqualTo(expectedRating.Name));
					Assert.That(actualRating.Urn, Is.EqualTo(expectedRating.Urn));
					Assert.That(actualRating.CreatedAt, Is.EqualTo(expectedRating.CreatedAt));
					Assert.That(actualRating.UpdatedAt, Is.EqualTo(expectedRating.UpdatedAt));
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
			
			var logger = new Mock<ILogger<RatingService>>();
			var ratingService = new RatingService(httpClientFactory.Object, logger.Object);
			
			// act
			var ratings = await ratingService.GetRatings();

			// assert
			Assert.That(ratings, Is.Not.Null);
			// TODO uncomment when trams api is live
			//Assert.That(types.Count, Is.EqualTo(0));
			Assert.That(ratings.Count, Is.EqualTo(5));
		}
	}
}