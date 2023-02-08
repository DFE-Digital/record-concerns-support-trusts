using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Trusts
{
	public interface ITrustService
	{
		string BuildRequestUri(TrustSearch trustSearch, int maxRecordsPerPage);
		Task<TrustSearchResponseDto> GetTrustsByPagination(TrustSearch trustSearch, int maxRecordsPerPage);
		Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn);
	}
}