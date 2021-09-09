using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}