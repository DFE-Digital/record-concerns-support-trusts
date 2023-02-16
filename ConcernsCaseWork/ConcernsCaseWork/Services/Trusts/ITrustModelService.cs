using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public interface ITrustModelService
	{
		Task<(TrustSearchModelPageResponseData PageData, IList<TrustSearchModel> Data)> GetTrustsBySearchCriteria(TrustSearch trustSearch);
		Task<TrustDetailsModel> GetTrustByUkPrn(string ukPrn);
		Task<TrustAddressModel> GetTrustAddressByUkPrn(string ukPrn);
	}
}