using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case;

public interface IGetClosedConcernsCaseSummariesByTrust
{
	Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters);
}

public class GetClosedConcernsCaseSummariesByTrust : IGetClosedConcernsCaseSummariesByTrust
{
	private readonly ICaseSummaryGateway _gateway;

	public GetClosedConcernsCaseSummariesByTrust(ICaseSummaryGateway gateway)
	{
		_gateway = gateway;
	}

	public async Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters)
	{
		(IList<ClosedCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.GetClosedCaseSummariesByTrust(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}