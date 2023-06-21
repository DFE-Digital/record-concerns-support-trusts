using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker, int page, int count);
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesForUsersTeam(string caseworker);
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker);
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn);
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn);
}