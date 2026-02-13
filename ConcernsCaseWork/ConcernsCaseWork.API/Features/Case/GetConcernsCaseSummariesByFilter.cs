using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case;

public interface IGetConcernsCaseSummariesByFilter
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByFilterParameters parameters);
}

public class GetConcernsCaseSummariesByFilter : IGetConcernsCaseSummariesByFilter
{
	private readonly ICaseSummaryGateway _gateway;

	public GetConcernsCaseSummariesByFilter(ICaseSummaryGateway gateway)
	{
		_gateway = gateway;
	}

	public async Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByFilterParameters parameters)
	{
		(IList<ActiveCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.GetCaseSummariesByFilter(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}