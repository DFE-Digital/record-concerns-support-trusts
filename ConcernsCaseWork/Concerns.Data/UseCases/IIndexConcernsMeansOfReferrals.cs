using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IIndexConcernsMeansOfReferrals
    {
        public IList<ConcernsMeansOfReferralResponse> Execute();
    }
}