using Service.TRAMS.Base;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch);
		Task<ApiWrapper<TrustSummaryDto>> GetTrustsByPagination(TrustSearch trustSearch);
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
	}
}