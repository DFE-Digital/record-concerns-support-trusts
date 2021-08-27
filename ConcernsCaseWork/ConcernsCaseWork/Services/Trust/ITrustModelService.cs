using ConcernsCaseWork.Models;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trust
{
	public interface ITrustModelService
	{
		Task<IList<TrustModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}