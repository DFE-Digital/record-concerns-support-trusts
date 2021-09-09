using Service.TRAMS.RecordSrma;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordSrma
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}