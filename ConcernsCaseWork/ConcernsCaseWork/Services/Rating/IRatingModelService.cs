using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Rating
{
	public interface IRatingModelService
	{
		Task<IList<RatingModel>> GetRatings();
	}
}