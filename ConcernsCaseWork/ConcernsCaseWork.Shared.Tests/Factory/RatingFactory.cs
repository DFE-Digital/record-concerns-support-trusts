using ConcernsCaseWork.Models;
using Service.TRAMS.Ratings;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RatingFactory
	{
		public static List<RatingDto> BuildListRatingDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new List<RatingDto>
			{
				new RatingDto("n/a", currentDate, currentDate, 1),
				new RatingDto("Red-Plus", currentDate, currentDate, 2),
				new RatingDto("Red", currentDate, currentDate, 3),
				new RatingDto("Red-Amber", currentDate, currentDate, 4),
				new RatingDto("Amber-Green", currentDate, currentDate, 5)
			};
		}

		public static RatingDto BuildRatingDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new RatingDto("Red-Plus", currentDate, currentDate, 2);
		}

		public static List<RatingModel> BuildListRatingModel()
		{
			return new List<RatingModel>
			{
				BuildRatingModel()
			};
		}

		public static RatingModel BuildRatingModel()
		{
			return new RatingModel
			{
				Name = "Red-Plus",
				Checked = true,
				Urn = 1,
				RagRating = new Tuple<int, IList<string>>(1, new List<string>() { "red" }),
				RagRatingCss = new List<string>() { "red" }
			};
		}
	}
}