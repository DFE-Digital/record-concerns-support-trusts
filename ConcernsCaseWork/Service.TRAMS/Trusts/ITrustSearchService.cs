using Service.TRAMS.RecordWhistleblower;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordWhistleblower
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}