using Concerns.Data.Models;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.Factories
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