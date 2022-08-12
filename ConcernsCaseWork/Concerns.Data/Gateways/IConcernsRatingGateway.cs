using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IConcernsRatingGateway
    {
        IList<ConcernsRating> GetRatings();
        ConcernsRating GetRatingByUrn(int urn);
    }
}