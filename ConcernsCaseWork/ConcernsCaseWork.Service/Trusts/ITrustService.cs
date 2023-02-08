using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Trusts
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch, int maxRecordsPerPage);
		Task<ApiListWrapper<TrustSearchDto>> GetTrustsByPagination(TrustSearch trustSearch, int maxRecordsPerPage);
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
	}
}