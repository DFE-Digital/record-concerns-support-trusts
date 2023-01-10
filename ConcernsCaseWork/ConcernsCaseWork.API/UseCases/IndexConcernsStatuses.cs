using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class IndexConcernsStatuses : IIndexConcernsStatuses
    {
        private readonly IConcernsStatusGateway _concernsStatusGateway;

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