using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> SearchActiveCaseSummaries(int? page = 1);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesForUsersTeam(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn, int? page);
	Task<CaseSummaryGroupModel<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn, int? page);
}