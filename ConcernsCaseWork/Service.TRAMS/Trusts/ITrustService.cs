using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch);
		Task<IList<TrustSummaryDto>> GetTrustsByPagination(TrustSearch trustSearch);
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
	}
}