using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}