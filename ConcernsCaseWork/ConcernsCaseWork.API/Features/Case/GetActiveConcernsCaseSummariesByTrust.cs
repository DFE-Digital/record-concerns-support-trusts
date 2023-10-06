using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case;

public interface IGetActiveConcernsCaseSummariesByTrust
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters);
}

public class GetActiveConcernsCaseSummariesByTrust : IGetActiveConcernsCaseSummariesByTrust
{
	private readonly ICaseSummaryGateway _gateway;

	public GetActiveConcernsCaseSummariesByTrust(ICaseSummaryGateway gateway)
	{
		_gateway = gateway;
	}

	public async Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters)
	{
		(IList<ActiveCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.GetActiveCaseSummariesByTrust(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}