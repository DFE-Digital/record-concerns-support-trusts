using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Rating
{
	public interface IRatingCachedService
	{
		Task<IList<RatingDto>> GetRatings();
	}
}