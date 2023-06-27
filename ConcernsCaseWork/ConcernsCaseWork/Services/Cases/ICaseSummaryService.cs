using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker);
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesForUsersTeam(string caseworker);
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker);
	
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn);
	
	Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn);
	
	Task<PagedCaseSummaryModel> GetActiveCaseSummariesByTrust(string trustUkPrn,int page, int count);
	Task<PagedCaseSummaryModel> GetClosedCaseSummariesByTrust(string trustUkPrn, int page, int count);
	
	
}