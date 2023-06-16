using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesByTrust
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesByTrustParameters parameters);
}