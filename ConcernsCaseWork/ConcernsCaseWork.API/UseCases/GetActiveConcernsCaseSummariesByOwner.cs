using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public class GetActiveConcernsCaseSummariesByOwner : IGetActiveConcernsCaseSummariesByOwner
{
	private readonly ICaseSummaryGateway _gateway;

	public GetActiveConcernsCaseSummariesByOwner(ICaseSummaryGateway gateway)
	{
		_gateway = gateway;
	}
	
	public async Task<IList<ActiveCaseSummaryResponse>> Execute(string userName)
	{
		var caseSummaries = await _gateway.GetActiveCaseSummaries(userName);
		
		return caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList();
	}
}