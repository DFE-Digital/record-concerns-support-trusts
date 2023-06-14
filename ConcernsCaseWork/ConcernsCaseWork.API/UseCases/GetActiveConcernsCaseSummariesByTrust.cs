using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

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