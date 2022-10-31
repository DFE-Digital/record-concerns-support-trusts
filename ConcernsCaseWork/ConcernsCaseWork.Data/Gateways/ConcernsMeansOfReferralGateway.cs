using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsMeansOfReferralGateway : IConcernsMeansOfReferralGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsMeansOfReferralGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public IList<ConcernsMeansOfReferral> GetMeansOfReferrals()
        {
            return _concernsDbContext
                .ConcernsMeansOfReferrals
                .AsNoTracking()
                .ToList();
        }

        public ConcernsMeansOfReferral GetMeansOfReferralByUrn(int urn)
        {
            return _concernsDbContext
                .ConcernsMeansOfReferrals
                .AsNoTracking()
                .SingleOrDefault(m => m.Urn == urn);
        }
        
        public ConcernsMeansOfReferral GetMeansOfReferralById(int id)
        {
            return _concernsDbContext
                .ConcernsMeansOfReferrals
                .AsNoTracking()
                .SingleOrDefault(m => m.Id == id);
        }
    }
}