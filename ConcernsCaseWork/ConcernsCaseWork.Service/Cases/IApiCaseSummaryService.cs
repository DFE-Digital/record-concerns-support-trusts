using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<CaseSearchParametersDto> GetCaseSearchCriterias();
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetAllCaseSummariesByFilter(
		Region[] regions = null,
		string[] caseOwners = null,
		string[] caseTeamLeaders = null,
		CaseStatus[] statuses = null, 
		int? page = 1);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker, int? page);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker, int? page);
	Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker, int? page);
	Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(string trustUkPrn, int? page);
	Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn, int? page);
}