using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
    public class ConcernsTypeResponseFactory
    {
        public static ConcernsTypeResponse Create(ConcernsType concernsType)
        {
            return new ConcernsTypeResponse
            {
                Name = concernsType.Name,
                Description = concernsType.Description,
                CreatedAt = concernsType.CreatedAt,
                UpdatedAt = concernsType.UpdatedAt,
                Urn = concernsType.Urn
            };
        }
    }
}