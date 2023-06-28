using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesByOwner
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters);
}