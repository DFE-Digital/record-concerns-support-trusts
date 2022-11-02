using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsRatingGateway
    {
        IList<ConcernsRating> GetRatings();
        ConcernsRating GetRatingByUrn(int urn);
    }
}