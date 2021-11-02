using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseSearchService
	{
		Task<IList<CaseDto>> GetCasesBySearchCriteria(CaseTrustSearch caseTrustSearch);
	}
}