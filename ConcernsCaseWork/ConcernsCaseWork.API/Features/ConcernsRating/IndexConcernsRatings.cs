using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.ConcernsRating
{
	public interface IIndexConcernsRatings
	{
		public IList<ConcernsRatingResponse> Execute();
	}

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