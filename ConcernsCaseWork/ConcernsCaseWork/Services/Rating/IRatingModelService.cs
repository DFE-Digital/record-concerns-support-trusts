using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Rating
{
	public interface IRatingModelService
	{
		Task<IList<RatingModel>> GetRatings();
		Task<RatingModel> GetRatingByUrn(long urn);
		Task<IList<RatingModel>> GetSelectedRatingsByUrn(long urn);
	}
}