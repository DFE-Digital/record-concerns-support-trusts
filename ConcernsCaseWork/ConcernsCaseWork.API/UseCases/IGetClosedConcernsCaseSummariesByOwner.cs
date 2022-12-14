using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetClosedConcernsCaseSummariesByOwner
{
	Task<IList<ClosedCaseSummaryResponse>> Execute(string userName);
}