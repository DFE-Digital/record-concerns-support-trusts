using Service.TRAMS.Rating;
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
	}
}