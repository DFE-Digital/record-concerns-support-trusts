using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker, int? page);
	Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker);
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(string trustUkPrn);
	Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn);
}