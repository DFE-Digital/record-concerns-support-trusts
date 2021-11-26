using Service.TRAMS.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseSearchService
	{
		Task<IList<CaseDto>> GetCasesByCaseTrustSearch(CaseTrustSearch caseTrustSearch);
		Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch);
		Task<IList<CaseDto>> GetCasesByPageSearch(PageSearch pageSearch);
		Task<IList<CaseHistoryDto>> GetCasesHistoryByCaseSearch(CaseSearch caseSearch);
	}
}