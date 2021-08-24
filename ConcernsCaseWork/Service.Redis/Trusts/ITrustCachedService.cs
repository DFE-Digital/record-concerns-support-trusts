using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Trusts
{
	public interface ITrustCachedService
	{
		Task<IList<TrustDto>> GetTrustsCached();
	}
}