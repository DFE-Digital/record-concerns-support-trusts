using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
	public class ConcernsRatingResponseFactory
    {
        public static ConcernsRatingResponse Create(ConcernsRating concernsRating)
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