using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case;

public interface IGetClosedConcernsCaseSummariesByOwner
{
	Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters);
}

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