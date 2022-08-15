using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public class IndexConcernsStatuses : IIndexConcernsStatuses
    {
        private IConcernsStatusGateway _concernsStatusGateway;

        public IndexConcernsStatuses(IConcernsStatusGateway concernsStatusGateway)
        {
            _concernsStatusGateway = concernsStatusGateway;
        }
        public IList<ConcernsStatusResponse> Execute()
        {
            var statuses = _concernsStatusGateway.GetStatuses();
            return statuses.Select(ConcernsStatusResponseFactory.Create).ToList();
        }
    }
}