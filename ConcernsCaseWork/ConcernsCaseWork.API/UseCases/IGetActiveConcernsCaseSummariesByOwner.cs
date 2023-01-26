using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesByOwner
{
	Task<IList<ActiveCaseSummaryResponse>> Execute(string userName);
}