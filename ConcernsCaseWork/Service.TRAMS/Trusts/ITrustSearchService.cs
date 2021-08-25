using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustSearchService
	{
		Task<IList<TrustDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}