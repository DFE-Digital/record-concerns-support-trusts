using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Ratings
{
	public interface IRatingService
	{
		Task<IList<RatingDto>> GetRatings();
	}
}