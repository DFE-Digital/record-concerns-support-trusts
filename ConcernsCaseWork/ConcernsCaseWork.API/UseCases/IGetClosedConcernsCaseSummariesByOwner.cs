using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetClosedConcernsCaseSummariesByOwner
{
	Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByOwnerParameters parameters);
}