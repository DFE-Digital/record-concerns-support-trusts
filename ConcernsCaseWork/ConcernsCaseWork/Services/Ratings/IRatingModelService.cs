using ConcernsCaseWork.Models;
using ConcernsCasework.Service.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Ratings
{
	public interface IRatingModelService
	{
		Task<IList<RatingDto>> GetRatings();
		Task<IList<RatingModel>> GetRatingsModel();
		Task<RatingModel> GetRatingModelByUrn(long urn);
		Task<IList<RatingModel>> GetSelectedRatingsModelByUrn(long urn);
	}
}