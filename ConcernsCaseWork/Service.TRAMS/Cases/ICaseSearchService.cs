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

	public class TestDataCaseSearchService : ICaseSearchService
	{
		// in memory collection
		public Task<IList<CaseDto>> GetCasesByCaseTrustSearch(CaseTrustSearch caseTrustSearch)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<CaseDto>> GetCasesByPageSearch(PageSearch pageSearch)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<CaseHistoryDto>> GetCasesHistoryByCaseSearch(CaseSearch caseSearch)
		{
			throw new System.NotImplementedException();
		}
	}

}