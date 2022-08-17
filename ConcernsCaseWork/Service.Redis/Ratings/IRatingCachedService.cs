using ConcernsCasework.Service.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Ratings
{
	public interface IRatingCachedService
	{
		Task ClearData();
		Task<IList<RatingDto>> GetRatings();
		Task<RatingDto> GetDefaultRating();
	}
}