using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public class IndexConcernsMeansOfReferrals : IIndexConcernsMeansOfReferrals
    {
        private IConcernsMeansOfReferralGateway _concernsMeansOfReferralGateway;

        public IndexConcernsMeansOfReferrals(IConcernsMeansOfReferralGateway concernsMeansOfReferralGateway)
        {
            _concernsMeansOfReferralGateway = concernsMeansOfReferralGateway;
        }
        public IList<ConcernsMeansOfReferralResponse> Execute()
        {
            var types = _concernsMeansOfReferralGateway.GetMeansOfReferrals();
            return types.Select(ConcernsMeansOfReferralResponseFactory.Create).ToList();
        }
    }
}