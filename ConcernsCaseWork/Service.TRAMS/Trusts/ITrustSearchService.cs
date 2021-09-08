using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}