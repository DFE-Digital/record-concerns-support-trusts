using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetClosedConcernsCaseSummariesByTrust
{
	Task<IList<ClosedCaseSummaryResponse>> Execute(string trustUkPrn);
}