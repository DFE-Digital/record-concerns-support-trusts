using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker);
	// Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworkers(IEnumerable<string> caseWorkers);
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker);
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn);
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn);
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesForTeamMembers(string caseworker);
}