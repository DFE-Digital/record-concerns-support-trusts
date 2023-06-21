namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker);
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker);
	Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker);
	Task<ActivePagedCasesDto> GetActiveCaseSummariesByTrust(string trustUkPrn, int page, int recordCount);
	Task<ClosedPagedCasesDto> GetClosedCaseSummariesByTrust(string trustUkPrn, int page, int recordCount);
}