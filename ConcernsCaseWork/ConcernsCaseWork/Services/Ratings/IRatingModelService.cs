using ConcernsCaseWork.Models;
using Service.TRAMS.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Ratings
{
	public interface IRatingModelService
	{
		Task<IList<RatingDto>> GetRatings();
		Task<IList<RatingModel>> GetRatingsModel();
		Task<RatingModel> GetRatingModelById(long id);
		Task<IList<RatingModel>> GetSelectedRatingsModelById(long id);
	}
}