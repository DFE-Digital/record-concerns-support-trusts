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
	
	public async Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters)
	{
		(IList<ClosedCaseSummaryVm> caseSummaries, int recordCount) = await _gateway.GetClosedCaseSummariesByOwner(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}