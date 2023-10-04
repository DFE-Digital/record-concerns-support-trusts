using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.ConcernsStatus
{
	public interface IIndexConcernsStatuses
	{
		public IList<ConcernsStatusResponse> Execute();
	}

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