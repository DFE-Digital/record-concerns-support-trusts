using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public class GetClosedConcernsCaseSummariesByOwner : IGetClosedConcernsCaseSummariesByOwner
{
	private readonly ICaseSummaryGateway _gateway;

	public GetClosedConcernsCaseSummariesByOwner(ICaseSummaryGateway gateway)
	{
		_gateway = gateway;
	}
	
	public async Task<IList<ClosedCaseSummaryResponse>> Execute(string userName)
	{
		var caseSummaries = await _gateway.GetClosedCaseSummariesByOwner(userName);
		
		return caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList();
	}
}