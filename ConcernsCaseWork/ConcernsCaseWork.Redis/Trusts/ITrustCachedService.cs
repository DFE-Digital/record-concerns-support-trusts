using ConcernsCaseWork.Service.Trusts;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Trusts
{
	public interface ITrustCachedService
	{
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
		Task ClearData();
	}
}