using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsMeansOfReferrals
    {
        public IList<ConcernsMeansOfReferralResponse> Execute();
    }
}