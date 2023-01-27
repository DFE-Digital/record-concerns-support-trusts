namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker);
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker);
	Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker);
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(string trustUkPrn);
	Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn);
}