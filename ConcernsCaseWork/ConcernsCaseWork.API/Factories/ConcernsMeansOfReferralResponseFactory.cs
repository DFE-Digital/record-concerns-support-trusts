using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
	public static class ConcernsMeansOfReferralResponseFactory
    {
        public static ConcernsMeansOfReferralResponse Create(ConcernsMeansOfReferral concernsMeansOfReferral)
        {
            return new ConcernsMeansOfReferralResponse
            {
                Name = concernsMeansOfReferral.Name,
                Description = concernsMeansOfReferral.Description,
                CreatedAt = concernsMeansOfReferral.CreatedAt,
                UpdatedAt = concernsMeansOfReferral.UpdatedAt,
                Id = concernsMeansOfReferral.Id
            };
        }
    }
}