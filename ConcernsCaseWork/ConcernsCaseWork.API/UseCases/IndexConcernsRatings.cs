using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.API.UseCases
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