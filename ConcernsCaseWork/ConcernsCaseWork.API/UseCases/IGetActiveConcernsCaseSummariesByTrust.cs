using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesByTrust
{
	Task<IList<ActiveCaseSummaryResponse>> Execute(string trustUkPrn);
}