using ConcernsCaseWork.Models;
using Service.TRAMS.RecordSrma;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public interface ITrustModelService
	{
		Task<IList<TrustSummaryModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
		Task<TrustDetailsModel> GetTrustByUkPrn(string ukPrn);
	}
}