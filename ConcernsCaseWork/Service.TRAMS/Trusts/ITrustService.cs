using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch);
		Task<IList<TrustDto>> GetTrustsByPagination(TrustSearch trustSearch);
	}
}