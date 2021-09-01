using ConcernsCaseWork.Models;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public interface ITrustModelService
	{
		Task<IList<TrustSummaryModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}