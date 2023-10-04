using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
	public class ConcernsStatusResponseFactory
    {
        public  static ConcernsStatusResponse Create(ConcernsStatus concernsStatus)
        {
            return new ConcernsStatusResponse
            {
                Name = concernsStatus.Name,
                CreatedAt = concernsStatus.CreatedAt,
                UpdatedAt = concernsStatus.UpdatedAt,
                Id = concernsStatus.Id
            };
        }
    }
}