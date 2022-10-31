using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsRatingsGateway : IConcernsRatingGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsRatingsGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public IList<ConcernsRating> GetRatings()
        {
            return _concernsDbContext.ConcernsRatings.ToList();
        }

        public ConcernsRating GetRatingByUrn(int urn)
        {
            return _concernsDbContext.ConcernsRatings.FirstOrDefault(r => r.Urn == urn);
        }
    }
}