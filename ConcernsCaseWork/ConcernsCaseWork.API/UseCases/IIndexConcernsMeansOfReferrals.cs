using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsMeansOfReferrals
    {
        public IList<ConcernsMeansOfReferralResponse> Execute();
    }
}