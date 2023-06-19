using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

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