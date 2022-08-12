using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IConcernsMeansOfReferralGateway
    {
        IList<ConcernsMeansOfReferral> GetMeansOfReferrals();
        ConcernsMeansOfReferral GetMeansOfReferralByUrn(int urn);
        ConcernsMeansOfReferral GetMeansOfReferralById(int id);
    }
}