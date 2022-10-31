using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsRatings
    {
        public IList<ConcernsRatingResponse> Execute();
    }
}