using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface ISearchConcernCases
	{
		Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(SearchCasesParameters parameters);
	}

	public class SearchConcernCases : ISearchConcernCases
	{
		private readonly ICaseSummaryGateway _gateway;

		public SearchConcernCases(ICaseSummaryGateway gateway)
		{
			_gateway = gateway;
		}

		public async Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(SearchCasesParameters parameters)
		{
			(IList<ActiveCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.SearchActiveCases(parameters);

			return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
		}
	}
}
