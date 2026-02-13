using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetAllCaseSummariesByFilter(int? page);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker, int? page);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker, int? page);
	Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker, int? page);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(string trustUkPrn, int? page);
	Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn, int? page);
}