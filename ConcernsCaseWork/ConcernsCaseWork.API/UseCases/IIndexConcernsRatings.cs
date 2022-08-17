using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsRatings
    {
        public IList<ConcernsRatingResponse> Execute();
    }
}