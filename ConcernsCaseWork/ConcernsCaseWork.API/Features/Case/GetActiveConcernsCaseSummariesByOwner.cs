using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case;

public interface IGetActiveConcernsCaseSummariesByOwner
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters);
}

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