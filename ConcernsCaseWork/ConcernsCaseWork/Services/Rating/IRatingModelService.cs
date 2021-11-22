using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Rating
{
	public interface IRatingModelService
	{
		Task<IList<RatingModel>> GetRatingsModel();
		Task<RatingModel> GetRatingModelByUrn(long urn);
		Task<IList<RatingModel>> GetSelectedRatingsModelByUrn(long urn);
	}
}