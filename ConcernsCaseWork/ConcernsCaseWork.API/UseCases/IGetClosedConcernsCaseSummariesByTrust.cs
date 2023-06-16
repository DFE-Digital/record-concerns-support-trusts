using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetClosedConcernsCaseSummariesByTrust
{
	Task<(IList<ClosedCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters);
}