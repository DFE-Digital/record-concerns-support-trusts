namespace ConcernsCaseWork.Service.Cases;

public interface IApiCaseSummaryService
{
	Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker);
}