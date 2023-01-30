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
	
	public async Task<IList<ClosedCaseSummaryResponse>> Execute(string trustUkPrn)
	{
		var caseSummaries = await _gateway.GetClosedCaseSummariesByTrust(trustUkPrn);
		return caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList();
	}
}