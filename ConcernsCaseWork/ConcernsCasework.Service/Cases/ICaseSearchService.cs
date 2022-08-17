using ConcernsCasework.Service.Base;

namespace ConcernsCasework.Service.Cases
{
	public interface ICaseSearchService
	{
		Task<IList<CaseDto>> GetCasesByCaseTrustSearch(CaseTrustSearch caseTrustSearch);
		Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch);
		Task<IList<CaseDto>> GetCasesByPageSearch(PageSearch pageSearch);
		Task<IList<CaseHistoryDto>> GetCasesHistoryByCaseSearch(CaseSearch caseSearch);
	}
}