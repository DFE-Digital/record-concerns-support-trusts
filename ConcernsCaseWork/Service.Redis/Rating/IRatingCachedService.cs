using Service.TRAMS.Rating;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Rating
{
	public interface IRatingCachedService
	{
		Task ClearData();
		Task<IList<RatingDto>> GetRatings();
		Task<RatingDto> GetRatingByName(string name);
	}
}