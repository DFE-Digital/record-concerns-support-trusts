using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Ratings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Ratings
{
	[Parallelizable(ParallelScope.All)]
	public class RatingModelServiceTests
	{
		[Test]
		public async Task WhenGetRatingsModel_Returns_RatingsModel()
		{
			// arrange
			var mockRatingCacheService = new Mock<IRatingCachedService>();
			var mockLogger = new Mock<ILogger<RatingModelService>>();
			
			var ratingsDto = RatingFactory.BuildListRatingDto();

			mockRatingCacheService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			// act
			var recordModelService = new RatingModelService(mockRatingCacheService.Object, mockLogger.Object);
			var ratingsModel = await recordModelService.GetRatingsModel();

			// assert
			Assert.That(ratingsModel, Is.Not.Null);
			Assert.That(ratingsModel.Count, Is.EqualTo(ratingsDto.Count - 1));
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();
			var ratingsModelSorted= ratingsDto.OrderBy(r => r.Name).Select(RatingMapping.MapDtoToModel).ToList();
			
			for (var index = 0; index < ratingsModel.Count; index++)
			{
				var actualRatingModel = ratingsModel.ElementAt(index);
				var expectedRatingModel = ratingsModelSorted.ElementAt(index);
				
				Assert.That(actualRatingModel.Checked, Is.EqualTo(expectedRatingModel.Checked));
				Assert.That(actualRatingModel.Name, Is.EqualTo(expectedRatingModel.Name));
				Assert.That(actualRatingModel.Urn, Is.EqualTo(expectedRatingModel.Urn));
				Assert.That(actualRatingModel.RagRating.Item1, Is.EqualTo(expectedRatingModel.RagRating.Item1));
				Assert.That(actualRatingModel.RagRating.Item2.Count, Is.EqualTo(expectedRatingModel.RagRating.Item2.Count));
				Assert.That(actualRatingModel.RagRatingCss.Count, Is.EqualTo(expectedRatingModel.RagRatingCss.Count));
			}
		}
		
		[Test]
		public async Task WhenGetSelectedRatingsModelByUrn_Returns_RatingsModel()
		{
			// arrange
			var mockRatingCacheService = new Mock<IRatingCachedService>();
			var mockLogger = new Mock<ILogger<RatingModelService>>();
			
			var ratingsDto = RatingFactory.BuildListRatingDto();
			
			// First element is filtered out
			var ratingDto = ratingsDto.ElementAt(2);

			mockRatingCacheService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			// act
			var recordModelService = new RatingModelService(mockRatingCacheService.Object, mockLogger.Object);
			var ratingsModel = await recordModelService.GetSelectedRatingsModelByUrn(ratingDto.Urn);

			// assert
			Assert.That(ratingsModel, Is.Not.Null);
			Assert.That(ratingsModel.Count, Is.EqualTo(ratingsDto.Count - 1));
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();
			var ratingsModelSorted= ratingsDto.OrderBy(r => r.Name).Select(RatingMapping.MapDtoToModel).ToList();
			
			for (var index = 0; index < ratingsModel.Count; index++)
			{
				var actualRatingModel = ratingsModel.ElementAt(index);
				var expectedRatingModel = ratingsModelSorted.ElementAt(index);
				
				Assert.That(actualRatingModel.Checked, Is.EqualTo(expectedRatingModel.Urn.CompareTo(ratingDto.Urn) == 0 || expectedRatingModel.Checked));
				Assert.That(actualRatingModel.Name, Is.EqualTo(expectedRatingModel.Name));
				Assert.That(actualRatingModel.Urn, Is.EqualTo(expectedRatingModel.Urn));
				Assert.That(actualRatingModel.RagRating.Item1, Is.EqualTo(expectedRatingModel.RagRating.Item1));
				Assert.That(actualRatingModel.RagRating.Item2.Count, Is.EqualTo(expectedRatingModel.RagRating.Item2.Count));
				Assert.That(actualRatingModel.RagRatingCss.Count, Is.EqualTo(expectedRatingModel.RagRatingCss.Count));
			}
		}
		
		[Test]
		public async Task WhenGetRatingModelByUrn_Returns_RatingModel()
		{
			// arrange
			var mockRatingCacheService = new Mock<IRatingCachedService>();
			var mockLogger = new Mock<ILogger<RatingModelService>>();
			
			var ratingsDto = RatingFactory.BuildListRatingDto();
			
			// First element is filtered out
			var ratingDto = ratingsDto.ElementAt(2);

			mockRatingCacheService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			// act
			var recordModelService = new RatingModelService(mockRatingCacheService.Object, mockLogger.Object);
			var ratingModel = await recordModelService.GetRatingModelByUrn(ratingDto.Urn);

			// assert
			Assert.That(ratingModel, Is.Not.Null);
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();
			var ratingsModelSorted= ratingsDto.OrderBy(r => r.Name).Select(RatingMapping.MapDtoToModel).ToList();
			
			var expectedRatingModel = ratingsModelSorted.ElementAt(1);
				
			Assert.That(ratingModel.Checked, Is.EqualTo(expectedRatingModel.Checked));
			Assert.That(ratingModel.Name, Is.EqualTo(expectedRatingModel.Name));
			Assert.That(ratingModel.Urn, Is.EqualTo(expectedRatingModel.Urn));
			Assert.That(ratingModel.RagRating.Item1, Is.EqualTo(expectedRatingModel.RagRating.Item1));
			Assert.That(ratingModel.RagRating.Item2.Count, Is.EqualTo(expectedRatingModel.RagRating.Item2.Count));
			Assert.That(ratingModel.RagRatingCss.Count, Is.EqualTo(expectedRatingModel.RagRatingCss.Count));
		}
	}
}