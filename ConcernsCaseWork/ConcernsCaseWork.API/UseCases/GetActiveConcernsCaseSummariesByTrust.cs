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
	
	public async Task<IList<ActiveCaseSummaryResponse>> Execute(string trustUkPrn)
	{
		var caseSummaries = await _gateway.GetActiveCaseSummariesByTrust(trustUkPrn);
		
		return caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList();
	}
}