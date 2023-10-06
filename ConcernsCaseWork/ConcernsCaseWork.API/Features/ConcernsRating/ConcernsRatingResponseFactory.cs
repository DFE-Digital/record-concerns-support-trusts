using ConcernsCaseWork.API.Contracts.Concerns;

namespace ConcernsCaseWork.API.Features.ConcernsRating
{
	public class ConcernsRatingResponseFactory
	{
		public static ConcernsRatingResponse Create(Data.Models.ConcernsRating concernsRating)
		{
			return new ConcernsRatingResponse
			{
				Name = concernsRating.Name,
				CreatedAt = concernsRating.CreatedAt,
				UpdatedAt = concernsRating.UpdatedAt,
				Id = concernsRating.Id
			};
		}
	}
}