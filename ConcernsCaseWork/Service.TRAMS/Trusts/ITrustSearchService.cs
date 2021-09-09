using Service.TRAMS.RecordAcademy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}