using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class RatingMappingTests
	{
		[TestCase(null, "-", 0)]
		[TestCase("", "n/a", 0)]
		[TestCase("n/a", "-", 0)]
		[TestCase("Amber-Green", "Amber,Green", 1)]
		[TestCase("Red-Amber", "Red,Amber", 2)]
		[TestCase("Red", "Red", 3)]
		[TestCase("Red-Plus", "Red Plus", 4)]
		[TestCase("Amber", "Amber", 5)]
		[TestCase("Green", "Green", 6)]
		public void WhenFetchRag_ReturnsListRags(string rating, string expected, int expectedOrder)
		{
			// arrange
			var splitExpected = expected.Split(",").ToList();
			
			// act
			var rags = RatingMapping.FetchRag(rating);

			// assert
			CollectionAssert.AreEqual(rags.Item2, splitExpected);
			Assert.That(rags.Item1, Is.EqualTo(expectedOrder));
		}
		
		[TestCase(null, "")]
		[TestCase("", "n/a")]
		[TestCase("n/a", "")]
		[TestCase("Red-Plus", "ragtag__redplus")]
		[TestCase("Red", "ragtag__red")]
		[TestCase("Red-Amber", "ragtag__red,ragtag__amber")]
		[TestCase("Amber-Green", "ragtag__amber,ragtag__green")]
		[TestCase("Amber", "ragtag__amber")]
		[TestCase("Green", "ragtag__green")]
		public void WhenFetchRagCss_ReturnsListRags(string rating, string expected)
		{
			// arrange
			var splitExpected = expected.Split(",").ToList();
			
			// act
			var rags = RatingMapping.FetchRagCss(rating);

			// assert
			CollectionAssert.AreEqual(rags, splitExpected);
		}

		[Test]
		public void WhenMapDtoToModel_ReturnsRatingModel()
		{
			// arrange
			var ratingDto = RatingFactory.BuildRatingDto();
			
			// act
			var ratingModel = RatingMapping.MapDtoToModel(ratingDto);
			
			// assert
			Assert.That(ratingModel, Is.Not.Null);
			Assert.That(ratingModel.Name, Is.Not.Null);
			Assert.That(ratingModel.Urn, Is.Not.Zero);
			Assert.That(ratingModel.RagRating, Is.Not.Null);
			Assert.That(ratingModel.RagRatingCss, Is.Not.Null);
		}

		[Test]
		public void WhenMapDtoToModel_When_Urn_Is_Unknown_ReturnsFirstRatingModel()
		{
			// arrange
			var uknownUrn = 0;
			var ratingDto = RatingFactory.BuildRatingDto();
			var ratingsDto = new List<RatingDto>() { ratingDto };

			// act
			var ratingModel = RatingMapping.MapDtoToModel(ratingsDto, uknownUrn);

			// assert
			Assert.That(ratingModel, Is.Not.Null);
			Assert.That(ratingModel.Name, Is.Not.Null);
			Assert.That(ratingModel.Urn, Is.Not.Zero);
			Assert.That(ratingModel.RagRating, Is.Not.Null);
			Assert.That(ratingModel.RagRatingCss, Is.Not.Null);
		}
	}
}