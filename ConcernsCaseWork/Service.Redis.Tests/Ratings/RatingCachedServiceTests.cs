using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Ratings;
using Service.TRAMS.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Ratings
{
	[Parallelizable(ParallelScope.All)]
	public class RatingCachedServiceTests
	{
		[Test]
		public async Task WhenClearData_ReturnSuccess()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRatingService = new Mock<IRatingService>();
			var mockLogger = new Mock<ILogger<RatingCachedService>>();

			var ratingCachedService = new RatingCachedService(mockCacheProvider.Object, mockRatingService.Object, mockLogger.Object);

			// act
			await ratingCachedService.ClearData();
		}

		[Test]
		public async Task WhenGetRatings_ReturnsRatings_FromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRatingService = new Mock<IRatingService>();
			var mockLogger = new Mock<ILogger<RatingCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>())).
				ReturnsAsync(RatingFactory.BuildListRatingDto());
			
			var ratingCachedService = new RatingCachedService(mockCacheProvider.Object, mockRatingService.Object, mockLogger.Object);

			// act
			var ratingsDto = await ratingCachedService.GetRatings();
			
			// assert
			Assert.That(ratingsDto, Is.Not.Null);
			Assert.That(ratingsDto.Count, Is.EqualTo(5));
			mockCacheProvider.Verify(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<RatingDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockRatingService.Verify(c => c.GetRatings(), Times.Never);
		}

		[Test]
		public async Task WhenGetRatings_ReturnsRatings_FromTramsApi()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRatingService = new Mock<IRatingService>();
			var mockLogger = new Mock<ILogger<RatingCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>())).
				Returns(Task.FromResult<IList<RatingDto>>(null));
			mockRatingService.Setup(r => r.GetRatings()).ReturnsAsync(RatingFactory.BuildListRatingDto());
			
			var ratingCachedService = new RatingCachedService(mockCacheProvider.Object, mockRatingService.Object, mockLogger.Object);

			// act
			var ratingsDto = await ratingCachedService.GetRatings();
			
			// assert
			Assert.That(ratingsDto, Is.Not.Null);
			Assert.That(ratingsDto.Count, Is.EqualTo(5));
			mockCacheProvider.Verify(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<RatingDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRatingService.Verify(c => c.GetRatings(), Times.Once);
		}
	}
}