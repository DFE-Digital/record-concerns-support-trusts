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
	
	public async Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters)
	{
		(IList<ActiveCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.GetActiveCaseSummariesByOwner(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}