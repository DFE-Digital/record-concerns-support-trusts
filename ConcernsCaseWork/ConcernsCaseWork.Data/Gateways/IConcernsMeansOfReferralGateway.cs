using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsMeansOfReferralGateway
    {
        IList<ConcernsMeansOfReferral> GetMeansOfReferrals();
        ConcernsMeansOfReferral GetMeansOfReferralById(int id);
    }
}