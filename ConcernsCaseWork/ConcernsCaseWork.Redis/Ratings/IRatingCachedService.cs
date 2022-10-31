using ConcernsCaseWork.Service.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Ratings
{
	public interface IRatingCachedService
	{
		Task ClearData();
		Task<IList<RatingDto>> GetRatings();
		Task<RatingDto> GetDefaultRating();
	}
}