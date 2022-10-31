using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Trusts
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch);
		Task<ApiListWrapper<TrustSearchDto>> GetTrustsByPagination(TrustSearch trustSearch);
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
	}
}