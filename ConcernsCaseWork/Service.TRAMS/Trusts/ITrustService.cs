using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface ITrustService
	{
		Task<IEnumerable<TrustDto>> GetTrustsByPagination(int page = 1);
	}
}