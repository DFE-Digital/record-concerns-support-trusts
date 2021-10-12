using Service.TRAMS.Trusts;
using System.Threading.Tasks;

namespace Service.Redis.Trusts
{
	public interface ITrustCachedService
	{
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
		Task ClearData();
	}
}