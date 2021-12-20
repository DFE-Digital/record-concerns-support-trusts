using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSearchDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}