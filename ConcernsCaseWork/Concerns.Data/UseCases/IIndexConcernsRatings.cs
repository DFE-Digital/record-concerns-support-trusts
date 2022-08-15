using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IIndexConcernsRatings
    {
        public IList<ConcernsRatingResponse> Execute();
    }
}