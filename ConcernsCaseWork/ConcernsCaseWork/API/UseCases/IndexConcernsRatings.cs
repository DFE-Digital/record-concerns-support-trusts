using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public class IndexConcernsRatings : IIndexConcernsRatings
    {
        private readonly IConcernsRatingGateway _concernsRatingGateway;

        public IndexConcernsRatings(IConcernsRatingGateway concernsRatingGateway)
        {
            _concernsRatingGateway = concernsRatingGateway;
        }

        public IList<ConcernsRatingResponse> Execute()
        {
            var ratings = _concernsRatingGateway.GetRatings();
            return ratings.Select(ConcernsRatingResponseFactory.Create).ToList();
        }
    }
}