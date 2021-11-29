using Service.TRAMS.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Ratings
{
	public interface IRatingCachedService
	{
		Task ClearData();
		Task<IList<RatingDto>> GetRatings();
	}
}