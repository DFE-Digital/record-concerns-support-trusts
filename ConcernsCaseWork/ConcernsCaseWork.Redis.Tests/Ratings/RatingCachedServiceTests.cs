using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.Ratings
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

		[Test]
		public async Task WhenGetDefaultRating_Return_Rating()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockRatingService = new Mock<IRatingService>();
			var mockLogger = new Mock<ILogger<RatingCachedService>>();

			var ratingsDto = RatingFactory.BuildListRatingDto();
			
			mockCacheProvider.Setup(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>())).
				Returns(Task.FromResult<IList<RatingDto>>(null));
			mockRatingService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			
			var ratingCachedService = new RatingCachedService(mockCacheProvider.Object, mockRatingService.Object, mockLogger.Object);

			// act
			var ratingDto = await ratingCachedService.GetDefaultRating();
			
			var expectedRating = ratingsDto.FirstOrDefault(r => r.Name.Equals("n/a"));
			
			// assert
			Assert.That(ratingDto, Is.Not.Null);
			Assert.That(expectedRating, Is.Not.Null);
			Assert.That(ratingDto.Name, Is.EqualTo(expectedRating.Name));
			Assert.That(ratingDto.Id, Is.EqualTo(expectedRating.Id));
			Assert.That(ratingDto.CreatedAt, Is.EqualTo(expectedRating.CreatedAt));
			Assert.That(ratingDto.UpdatedAt, Is.EqualTo(expectedRating.UpdatedAt));
			
			mockCacheProvider.Verify(c => c.GetFromCache<IList<RatingDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<RatingDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockRatingService.Verify(c => c.GetRatings(), Times.Once);
		}
	}
}