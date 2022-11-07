using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public interface ITrustModelService
	{
		Task<IList<TrustSearchModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
		Task<TrustDetailsModel> GetTrustByUkPrn(string ukPrn);
		Task<TrustAddressModel> GetTrustAddressByUkPrn(string ukPrn);
	}
}